using MongoDB.Bson;
using MonitoringClient.Prometheus.PrometheusApi;
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
        private readonly PrometheusClient _prometheusClient;
        private const string INTERVAL_FILTER = "1m";

        public RecommendationsService(MongoDBService mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            _prometheusClient = new PrometheusClient();
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
                double avgCpuUsageUtilization = 80;// _prometheusClient.GetAvgCpuUsageUtilization(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgCpuUsageUtilizationPercentage = avgCpuUsageUtilization / 100;

                return (int)Math.Ceiling(instance.Specifications.VCPU * avgCpuUsageUtilizationPercentage);
            };
        }

        private Func<InstanceDetails, object> SetMemoryThresholdMethod()
        {
            return (instance) =>
            {
                // maybe get the usage property instead of send request to promethiues
                double avgFreeMemorySizeInGB = 70;//_prometheusClient.GetAvgFreeMemorySizeInGB(instance.InstanceAddress, INTERVAL_FILTER).Result;
                double avgFreeMemorySizeInGBPercentage = avgFreeMemorySizeInGB / 100;

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
