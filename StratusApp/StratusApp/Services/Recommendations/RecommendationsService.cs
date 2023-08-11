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
        private const string INTERVAL_FILTER = "month";

        public RecommendationsService(MongoDBService mongoDatabase, CollectorService collectorService)
        {
            _mongoDatabase = mongoDatabase;
            _collectorService = collectorService;
            _instanceFilters = new InstanceFilter();

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
                avgCpuUsageUtilizationPercentage = avgCpuUsageUtilizationPercentage == 0 ? 0 : avgCpuUsageUtilizationPercentage;

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
                avgFreeMemorySizeInGBPercentage = avgFreeMemorySizeInGBPercentage == 0 ? 0 : avgFreeMemorySizeInGBPercentage;

                return Math.Ceiling(instance.Specifications.Memory.Value * avgFreeMemorySizeInGBPercentage);
            };
        }

        public async Task<List<CustomInstances>> GetRecommendationsInstances()
        {
            //get only user instances !
            var userInstances = _mongoDatabase.GetCollectionAsList<AwsInstanceDetails>(eCollectionName.Instances).Result;
            List<CustomInstances> customInstances = new List<CustomInstances>();

            foreach (var userInstance in userInstances)
            {
                var optionalInstances = _mongoDatabase.GetDocuments(eCollectionName.AlternativeInstances, _instanceFilters.Filter(userInstance)).Result;

                if(userInstance.Specifications.Storage == null) userInstance.Specifications.Storage = new Storage(); // Delete after db change storage val
                
                foreach (var instance in optionalInstances)
                {
                    if (instance.Region == "US East (N. Virginia)" && instance.InstanceType == userInstance.Type) continue; //TODO change to user region

                    customInstances.Add(new CustomInstances(userInstance, instance));
                }
            }

            return customInstances;
        }
    }
}
