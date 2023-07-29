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

        internal async Task<List<AwsInstanceDetails>?> GetInstanceFormalData()
        {
            var instances = await _cloudApiClient.GetInstanceFormalData();

            //SetPriceFromDB(instances);
            InsertUserInstancesToDB(instances);
            
            return instances;
        }

        private void SetPriceFromDB(List<AwsInstanceDetails> instances)
        {
            foreach (var instance in instances)     
            {
                string type = instance.Type;

                List<AlternativeInstance> filteredInstances = _mongoDBService.GetDocuments<AlternativeInstance>(eCollectionName.AlternativeInstances,
                    (alternativeInstance) => alternativeInstance.InstanceType == type && alternativeInstance.region == _cloudApiClient.Region.DisplayName).Result;
                if(filteredInstances.Count == 1)
                {
                    //instance.Price = filteredInstances.ElementAt(0).HourlyRate.;
                }
            }
        }

        private void InsertUserInstancesToDB(List<AwsInstanceDetails> instances)
        {
            //Get DB instances here to prevent redundant DB calls
            var dBInstances = _mongoDBService.GetCollectionAsList<AwsInstanceDetails>(eCollectionName.Instances).Result;
            foreach (AwsInstanceDetails instance in instances)
            {
                if(!IsInstanceExistsInDB(instance, dBInstances))
                {
                    _mongoDBService.InsertDocument<AwsInstanceDetails>(eCollectionName.Instances, instance);
                }
            }
        }

        private bool IsInstanceExistsInDB(AwsInstanceDetails instance, List<AwsInstanceDetails> dBInstances)
        {  
            bool result = false;

            foreach(AwsInstanceDetails instanceDetails in dBInstances)
            {
               
                if(instanceDetails != null && instanceDetails.InstanceId.Equals(instance.InstanceId) && instanceDetails.InstanceAddress.Equals(instance.InstanceAddress))
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
            List<AlternativeInstance>instances = await _cloudApiClient.ScrapeInstancesDetails();
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
