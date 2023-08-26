using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using CloudApiClient.AwsServices.AwsUtils;
using MonitoringClient;
using MonitoringClient.Prometheus;
using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusApi;
using MonitoringClient.Prometheus.PrometheusModels;
using MonitoringClient.Prometheus.PrometheusModels.OldPrometheusModels_bkup;
using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
using StratusApp.Services.MongoDBServices;
using System;
using System.Reflection;
using Utils.DTO;
using Utils.Enums;
using static Amazon.EC2.Util.VPCUtilities;
using AwsClient = CloudApiClient.CloudApiClient;

namespace StratusApp.Services.Collector
{
    public class CollectorService : ICollectorService
    {
        private readonly PrometheusClient _prometheusClient;
        private readonly CollectorUtils _collectorUtils;
        private readonly MongoDBService _mongoDBService;
        private readonly AwsService _awsService;
        //private readonly AwsClient _awsClient;

        public CollectorService(MongoDBService mongoDBService, EC2ClientFactory ec2ClientFactory)
        {
            this._mongoDBService = mongoDBService; 
            _prometheusClient = new PrometheusClient();
            _collectorUtils = new CollectorUtils();
            _awsService = new AwsService(mongoDBService, ec2ClientFactory);
        }

        public async Task<List<CpuUsageData>> GetAvgCpuUsageUtilizationOverTime(string instanceAddr, string timePeriodStr)
        {
            List<CpuUsageData> cpuUsageDataList; // = new List<CpuUsageData>();
            PrometheusQueryParams queryParams = new PrometheusQueryParams();
            QueryOverTimePeriod overallTimePeriod;
            QueryOverTimePeriod queryStep;
            DateTime endTime = DateTime.UtcNow;
            DateTime startTime;

            overallTimePeriod = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timePeriodStr);
            (startTime, queryStep) = _collectorUtils.BuildTimeRangeQueryByTimePeriod(overallTimePeriod, endTime);

            // Fill queryParams
            queryParams.InstanceAddr = instanceAddr;
            queryParams.ExpressionQuery = PrometheusExpressionQueryType.RangeQuery;
            queryParams.OverTimeFilter = queryStep;
            queryParams.QueryType = PrometheusQueryType.GetAvgCpuUsageUtilizationOverTime;
            queryParams.StartTime = startTime;
            queryParams.EndTime = endTime;
            queryParams.QueryStep = queryStep;

            var result = await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndValuesList>(queryParams);
            cpuUsageDataList = _collectorUtils.FillCpuUsageDataList(startTime, endTime, queryStep, result.TimestampsAndValues);
            
            return cpuUsageDataList;

        }

        public async Task<double> GetAvgCpuUsageUtilization(string instance, string timeFilter)
        {
            double avgCpuUsageUtilization;

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgCpuUsageUtilization
            };


            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);
            

            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;

        }

        public async Task<List<SingleCpuUtilizationDTO>?> GetAvgCpuUtilizationByCpu(string instance, string timeFilter)
        {
            List<SingleCpuUtilizationDTO> cpusUtilizationDTOs = new List<SingleCpuUtilizationDTO>();

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgCpuUtilizationByCpu
            };

            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<List<CpuMetricAndSingleValue>>(queryParams);


            foreach (CpuMetricAndSingleValue currCpu in promResult)
            {
                int cpuIdx = int.Parse(currCpu.Metric.Cpu);
                double cpuUtilizationPercentage = double.Parse(currCpu.TimestampsAndValues[1]);
                cpusUtilizationDTOs.Add(new SingleCpuUtilizationDTO
                {
                    CpuNo = ++cpuIdx,
                    UtilizationPercentage = cpuUtilizationPercentage
                });
            }

            if (cpusUtilizationDTOs.Count > 0)
            {
                SetUtilizationPercentageByVCpu(cpusUtilizationDTOs); 
            }

            return cpusUtilizationDTOs;
        }

        private void SetUtilizationPercentageByVCpu(List<SingleCpuUtilizationDTO> cpusUtilizationDTOs)
        {
            int totalCpuCapacity = cpusUtilizationDTOs.Count * 100;
            double totalCpuUsage = 0;

            foreach(SingleCpuUtilizationDTO singleCpuUtilization in cpusUtilizationDTOs)
            {
                singleCpuUtilization.UtilizationPercentage = singleCpuUtilization.UtilizationPercentage / totalCpuCapacity * 100;
                singleCpuUtilization.UtilizationPercentage = Math.Round(singleCpuUtilization.UtilizationPercentage, 3);
                totalCpuUsage += singleCpuUtilization.UtilizationPercentage;
            }

            cpusUtilizationDTOs.Add(new SingleCpuUtilizationDTO()
            {
                CpuNo = cpusUtilizationDTOs.Count + 1,
                UtilizationPercentage = 100 - totalCpuUsage,
                IsFreeSpaceValue = true,
            });
        }

        public async Task<double> GetAvgFreeDiskSpaceInGB(string instance, string timeFilter)
        {

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgFreeDiskSpaceInGB
            };

            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgFreeMemorySizeInGB(string instance, string timeFilter)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgFreeMemorySizeInGB
            };

            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;
        }

        public async Task<InstanceDetailsDTO> GetInstanceSpecifications(string instanceAddr)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetNumberOfvCPU(string instance)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                QueryType = PrometheusQueryType.GetNumberOfvCPU
                
            };

            var result = 
                await _prometheusClient.ExecutePromQLQuery<List<CpuMetricAndSingleValue>>(queryParams);

            return result.Count;
        }

        public async Task<double> GetTotalDiskSizeInGB(string instance)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                QueryType = PrometheusQueryType.GetTotalDiskSizeInGB
            };

            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetTotalMemorySizeInGB(string instance)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                QueryType = PrometheusQueryType.GetTotalMemorySizeInGB
            };

            var promResult = 
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);
            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetMaxCpuUsageUtilization(string instance, string timeFilter)
        {
            double avgCpuUsageUtilization;

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetMaxCpuUsageUtilization
            };

            var promResult =
                await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            double result = double.Parse(promResult.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgDiskSpaceUsageInGB(string instance, string timeFilter)
        {
            double avgDiskSpaceUsage;

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgDiskSpaceUsageInGB
            };

            var promResult = await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            avgDiskSpaceUsage = double.Parse(promResult.TimestampAndValue[1]);

            return avgDiskSpaceUsage;
        }

        public async Task<double> GetAvgMemorySizeUsageInGB(string instance, string timeFilter)
        {
            double avgMemoryUsage;

            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                OverTimeFilter = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timeFilter),
                QueryType = PrometheusQueryType.GetAvgMemorySizeUsageInGB
            };

            var promResult = await _prometheusClient.ExecutePromQLQuery<EmptyMetricAndSingleValue>(queryParams);

            avgMemoryUsage = double.Parse(promResult.TimestampAndValue[1]);

            return avgMemoryUsage;
        }

        public async Task<double> GetAvgDiskSpaceUsagePercentage(string instance, string timeFilter)
        {
            double averageDiskSpaceUsagePercentage;

            double avgUsage = await GetAvgDiskSpaceUsageInGB(instance, timeFilter);
            double totalSize = await GetTotalDiskSizeInGB(instance);

            averageDiskSpaceUsagePercentage = (avgUsage / totalSize) * 100;
            return averageDiskSpaceUsagePercentage;
        }

        public async Task<double> GetAvgMemorySizeUsagePercentage(string instance, string timeFilter)
        {
            double averageMemoryUsagePercentage;

            double avgUsage = await GetAvgMemorySizeUsageInGB(instance, timeFilter);
            double totalSize = await GetTotalMemorySizeInGB(instance);

            averageMemoryUsagePercentage = (avgUsage / totalSize) * 100;
            return averageMemoryUsagePercentage;
        }

        public async Task<List<AwsInstanceDetails>> GetAllUserResourcesDetails(string userEmail)
        {
            /**
             * get Type (t2.micro)
             * get OS
             * get Price
             * get CPU
             * get Storage
             * get Memory
             * 
             *
             */

            // Collect all user instances - by userEmail
       /*     var instances = _mongoDBService.GetDocuments<AwsInstanceDetails>(eCollectionName.Instances,
                (AwsInstanceDetails insDetails) => insDetails.UserEmail == userEmail
                );*/

            // Aws Side : Filling InstanceId, InstanceAddr, Type, Price - currently No
            List<AwsInstanceDetails> instanceDetailsList = await _awsService.GetBasicAwsInstancesDetails();

           
            // Complete procedure by getting the right specs from Prometheus
            foreach (AwsInstanceDetails singleInstanceDetails in instanceDetailsList)
            {
                string instaceAddr = singleInstanceDetails.InstanceAddress;

                //OS
                singleInstanceDetails.Specifications.OperatingSystem = "Linux";

                // Memory
                double totalMemSize = await GetTotalMemorySizeInGB(instaceAddr);
                Memory memory = new Memory(totalMemSize, eSizeUnit.GB);
                singleInstanceDetails.Specifications.Memory = memory;

                // Storage
                double totalStorageSize = await GetTotalDiskSizeInGB(instaceAddr);
                Storage storage = new Storage(totalStorageSize, eSizeUnit.GB);
                singleInstanceDetails.Specifications.Storage = storage;

                // vCpu
                int numOfVCpus = await GetNumberOfvCPU(instaceAddr);
                singleInstanceDetails.Specifications.VCPU = numOfVCpus;
            }

            // Update DB with all the data collected.
            _awsService.InsertUserInstancesToDB(instanceDetailsList);

            return instanceDetailsList;
        }
    }
}
