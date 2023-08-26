using MongoDB.Bson;
using MonitoringClient.Prometheus.PrometheusApi;
using StratusApp.Services.Collector;
using StratusApp.Services.MongoDBServices;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using Utils.DTO;

namespace StratusApp.Services.Recommendations
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly MongoDBService _mongoDatabase;
        private readonly InstanceFilter _instanceFilters;
        private readonly CollectorService _collectorService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string INTERVAL_FILTER = "month";

        public RecommendationsService(MongoDBService mongoDatabase, CollectorService collectorService, IHttpContextAccessor httpContextAccessor)
        {
            _mongoDatabase = mongoDatabase;
            _collectorService = collectorService;
            _instanceFilters = new InstanceFilter();
            _httpContextAccessor = httpContextAccessor;

            InitValuesFilter();
        }

        private void InitValuesFilter()
        {
            _instanceFilters.AddFilterValue(FilterField.Memory, SetMemoryThresholdMethod());
            _instanceFilters.AddFilterValue(FilterField.VCPU, SetVCPUThresholdMethod());
            //_instanceFilters.AddFilterValue(FilterField.Storage, SetStorageThresholdMethod()); TODO
        }

        private Func<InstanceDetails, object> SetVCPUThresholdMethod()
        {
            return (instance) =>
            {
                // maybe get the usage property instead of send request to promethiues
                double avgCpuUsageUtilization = _collectorService.GetAvgCpuUsageUtilization(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgCpuUsageUtilizationPercentage = avgCpuUsageUtilization / 100;
                avgCpuUsageUtilizationPercentage = avgCpuUsageUtilizationPercentage == 0 ? instance.Specifications.VCPU : avgCpuUsageUtilizationPercentage;

                return (int)Math.Ceiling(instance.Specifications.VCPU * avgCpuUsageUtilizationPercentage);
            };
        }

        private Func<InstanceDetails, object> SetMemoryThresholdMethod()
        {
            return (instance) =>
            {
                // maybe get the usage property instead of send request to promethiues
                double avgFreeMemorySizeInGB = _collectorService.GetAvgFreeMemorySizeInGB(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgFreeMemorySizeInGBPercentage = avgFreeMemorySizeInGB / 100;
                avgFreeMemorySizeInGBPercentage = avgFreeMemorySizeInGBPercentage == 0 ? instance.Specifications.Memory.Value : avgFreeMemorySizeInGBPercentage;

                return Math.Ceiling(instance.Specifications.Memory.Value * avgFreeMemorySizeInGBPercentage);
            };
        }

        public async Task<List<CustomInstances>> GetInstancesRecommendation()
        {
            //TODO get only user instances !
            string userEmail = _httpContextAccessor.HttpContext.Request.Cookies["Stratus"];
            var user = _mongoDatabase.GetDocuments<StratusUser>(eCollectionName.Users, (StratusUser user) => user.Email.Equals(userEmail)).Result.FirstOrDefault();
            var userInstances = _mongoDatabase.GetDocuments<AwsInstanceDetails>(eCollectionName.Instances, (AwsInstanceDetails userInstances) => userInstances.UserEmail == user.Email).Result;
            List<CustomInstances> customInstances = new List<CustomInstances>();

            foreach (var userInstance in userInstances)
            {
                try
                {
                    var optionalInstances = _mongoDatabase.GetDocuments(eCollectionName.AlternativeInstances, _instanceFilters.Filter(userInstance)).Result;

                    if (userInstance.Specifications.Storage == null) userInstance.Specifications.Storage = new Storage(); // Delete after db change storage val

                    foreach (var instance in optionalInstances)
                    {
                        if (instance.Region == user.Region && instance.InstanceType == userInstance.Type) continue; //TODO change to user region

                        customInstances.Add(new CustomInstances(userInstance, instance));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }   
            }

            return customInstances;
        }
    }
}
