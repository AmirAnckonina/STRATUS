using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using StratusApp.Models;
using StratusApp.Services.MongoDBServices;
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
            foreach(InstanceDetails instance in instances)
            {
                if(!IsInstanceExistsInDB(instance))
                {
                    _mongoDBService.InsertDocument<InstanceDetails>(eCollectionName.Instances, instance);
                }
            }
        }

        private bool IsInstanceExistsInDB(InstanceDetails instance)
        {
            var instances = _mongoDBService.GetCollectionAsList<InstanceDetails>(eCollectionName.Instances).Result;
            bool result = false;

            foreach(InstanceDetails instanceDetails in instances)
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

        internal async Task<List<AlternativeInstance>?> ScrapeInstancesDetails()
        {
            return await _cloudApiClient.ScrapeInstancesDetails();
        }
    }
}
