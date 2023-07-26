using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using StratusApp.Models;
using StratusApp.Services.MongoDBServices;
using System.Collections.Generic;
using Utils.DTO;
using AwsClient = CloudApiClient.CloudApiClient;

namespace StratusApp.Services
{
    public class AwsService
    {
        private readonly MongoDBService _mongoDBService;
        private readonly AwsClient _cloudApiClient;
        private const string INSTANCES_COLLECTION = "Instances";

        public AwsService(MongoDBService mongoDatabase)
        {
            _mongoDBService = mongoDatabase;
            _cloudApiClient = new AwsClient();
        }

        internal async Task<double> GetCurrentInstanceVolumesUsage(string instanceId)
        {
            return await _cloudApiClient.GetCurrentInstanceVolumesUsage(instanceId);
        }

        internal async Task<InstanceDetails?> GetInstanceBasicDetails(string instanceId)
        {
            return await _cloudApiClient.GetInstanceBasicDetails(instanceId);
        }

        internal async Task<List<Datapoint>?> GetInstanceCPUStatistics(string instanceId)
        {
            return await _cloudApiClient.GetInstanceCPUStatistics(instanceId);
        }

        internal async Task<List<CpuUsageData>?> GetInstanceCpuUsageOverTime(string instanceId, string filterTime)
        {
            return await _cloudApiClient.GetInstanceCpuUsageOverTime(instanceId, filterTime);
        }

        internal async Task<List<InstanceDetails>?> GetInstanceFormalData()
        {
            var instances = await _cloudApiClient.GetInstanceFormalData();

            InsertUserInstancesToDB(instances);
            
            return instances;
        }

        private void InsertUserInstancesToDB(List<InstanceDetails> instances)
        {
            //Get DB instances here to prevent redundant DB calls
            var dBInstances = _mongoDBService.GetCollectionAsList<InstanceDetails>(eCollectionName.Instances).Result;
            foreach (InstanceDetails instance in instances)
            {
                if(!IsInstanceExistsInDB(instance, dBInstances))
                {
                    _mongoDBService.InsertDocument<InstanceDetails>(eCollectionName.Instances, instance);
                }
            }
        }

        private bool IsInstanceExistsInDB(InstanceDetails instance, List<InstanceDetails> dBInstances)
        {  
            bool result = false;

            foreach(InstanceDetails instanceDetails in dBInstances)
            {
                if(instanceDetails != null && instanceDetails.InstanceId.Equals(instance.InstanceId) && instanceDetails.IP.Equals(instance.IP))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        internal async Task<string?> GetInstanceOperatingSystem(string instanceId)
        {
            return await _cloudApiClient.GetInstanceOperatingSystem(instanceId);
        }

        internal async Task<StratusUser?> GetInstances()
        {
            return await _cloudApiClient.GetInstances();
        }

        internal async Task<int> GetInstanceTotalVolumesSize(string instanceId)
        {
            return await _cloudApiClient.GetInstanceTotalVolumesSize(instanceId); 
        }

        internal async Task<List<Volume>?> GetInstanceVolumes(string instanceId)
        {
            return await _cloudApiClient.GetInstanceVolumes(instanceId);
        }

        internal async Task<List<InstanceDetails>?> GetMoreFittedInstances(string instanceId)
        {
            return await _cloudApiClient.GetMoreFittedInstances(instanceId);
        }

        internal async Task<List<AlternativeInstance>?> ScrapeInstancesDetailsIntoDB()
        {
            List < AlternativeInstance >instances = await _cloudApiClient.ScrapeInstancesDetails();
            InsertAlternativeInstancesToDB(instances);
            return instances;
        }

        private void InsertAlternativeInstancesToDB(List<AlternativeInstance> instances)
        {
            var DBAlternativeinstances = _mongoDBService.GetCollectionAsList<AlternativeInstance>(eCollectionName.AlternativeInstances).Result;
            foreach (AlternativeInstance instance in instances)
            {
                if (!IsAlternativeInstanceExistsInDB(instance, DBAlternativeinstances))
                {
                    _mongoDBService.InsertDocument<AlternativeInstance>(eCollectionName.AlternativeInstances, instance);
                }
            }
            throw new NotImplementedException();
        }

        private bool IsAlternativeInstanceExistsInDB(AlternativeInstance instance, List<AlternativeInstance> DBAlternativeInstances)
        {
            bool result = false;

            foreach (AlternativeInstance alternativeInstance in DBAlternativeInstances)
            {
                if (alternativeInstance != null && alternativeInstance.InstanceType.Equals(instance.InstanceType) && alternativeInstance.region.Equals(instance.region))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
