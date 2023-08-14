using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using CloudApiClient.AwsServices.AwsUtils;
using StratusApp.Models;
using StratusApp.Services.MongoDBServices;
using System.Collections.Generic;
using Utils.DTO;
using Utils.Enums;
using AwsClient = CloudApiClient.CloudApiClient;

namespace StratusApp.Services
{
    public class AwsService
    {
        private readonly MongoDBService _mongoDBService;
        private readonly AwsClient _cloudApiClient;
        private const string INSTANCES_COLLECTION = "Instances";

        public AwsService(MongoDBService mongoDatabase, EC2ClientFactory ec2ClientFactory)
        {
            _mongoDBService = mongoDatabase;
            _cloudApiClient = new AwsClient(ec2ClientFactory);
        }
        internal bool StoreAWSCredentialsInSession(string accessKey, string secretKey, string region)
        {
            return _cloudApiClient.StoreAWSCredentialsInSession(accessKey, secretKey, region);
        }
        internal Dictionary<eAWSCredentials, string> GetAWSCredentialsFromSession()
        {
            return _cloudApiClient.GetAWSCredentialsFromSession();
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

        internal async Task<List<AwsInstanceDetails>> GetInstanceFormalData()
        {
            var instances = await _cloudApiClient.GetInstanceFormalData();

            SetPriceFromToInstances(instances);
            InsertUserInstancesToDB(instances);
            
            return instances;
        }

        private void SetPriceFromToInstances(List<AwsInstanceDetails> instances)
        {
            foreach (var instance in instances)     
            {
                string type = instance.Type;

                List<AlternativeInstance> filteredInstances = _mongoDBService.GetDocuments<AlternativeInstance>(eCollectionName.AlternativeInstances,
                (alternativeInstance) => alternativeInstance.InstanceType == type && alternativeInstance.Region == _cloudApiClient.Region.DisplayName).Result;
                
                if(filteredInstances.Count == 1)
                {
                    instance.Specifications.Price = filteredInstances.ElementAt(0).Specifications.Price;
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

        internal async Task<List<Instance>?> GetInstances()
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

        internal async Task<List<AwsInstanceDetails>> GetMoreFittedInstances(string instanceId)
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
            //try
            //{
            //    _mongoDBService.InsertMultipleDocuments<AlternativeInstance>(eCollectionName.AlternativeInstances, instances);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            foreach (AlternativeInstance instance in instances)
            {
                if (!IsAlternativeInstanceExistsInDB(instance, DBAlternativeinstances))
                {
                    _mongoDBService.InsertDocument<AlternativeInstance>(eCollectionName.AlternativeInstances, instance);
            
                }
            }
        }

        private bool IsAlternativeInstanceExistsInDB(AlternativeInstance instance, List<AlternativeInstance> DBAlternativeInstances)
        {
            bool result = false;

            foreach (AlternativeInstance alternativeInstance in DBAlternativeInstances)
            {
                if (alternativeInstance != null && alternativeInstance.InstanceType.Equals(instance.InstanceType) && alternativeInstance.Region.Equals(instance.Region))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
