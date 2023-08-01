using MonitoringClient;
using MonitoringClient.Prometheus;
using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusApi;
using MonitoringClient.Prometheus.PrometheusModels;
using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
using System;
using System.Reflection;
using Utils.DTO;
using Utils.Enums;
using static Amazon.EC2.Util.VPCUtilities;

namespace StratusApp.Services.Collector
{
    public class CollectorService : ICollectorService
    {
        private readonly PrometheusClient _prometheusClient;
        private readonly CollectorUtils _collectorUtils;

        public CollectorService()
        {
            _prometheusClient = new PrometheusClient();
            _collectorUtils = new CollectorUtils();
        }

        public async Task<List<CpuUsageData>> GetAvgCpuUsageUtilizationOverTime(string instanceAddr, string timePeriodStr)
        {
            List<CpuUsageData> cpuUsageDataList = new List<CpuUsageData>();
            PrometheusQueryParams queryParams = new PrometheusQueryParams();
            QueryOverTimePeriod overallTimePeriod;
            QueryOverTimePeriod queryStep;
            DateTime endTime = DateTime.UtcNow;
            DateTime startTime;

            overallTimePeriod = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timePeriodStr);

            //export to method inCollectorUtils.
            switch (overallTimePeriod)
            {
                case QueryOverTimePeriod.Day:
                   startTime = endTime.AddDays(-1);
                   queryStep = QueryOverTimePeriod.Hour;
                   break;

                case QueryOverTimePeriod.Year:
                   startTime = endTime.AddYears(-1);
                   queryStep = QueryOverTimePeriod.Month;
                   break;

                case QueryOverTimePeriod.Month:
                case QueryOverTimePeriod.None:
                    startTime = endTime.AddMonths(-1);
                    queryStep = QueryOverTimePeriod.Day;
                    break;

                default:
                    throw new Exception("Query overall time isn't supported.");
            }

            // Fill queryParams
            queryParams.InstanceAddr = instanceAddr;
            queryParams.ExpressionQuery = PrometheusExpressionQueryType.RangeQuery;
            queryParams.OverTimeFilter = queryStep;
            queryParams.QueryType = PrometheusQueryType.GetAvgCpuUsageUtilizationOverTime;
            queryParams.StartTime = startTime;
            queryParams.EndTime = endTime;
            queryParams.QueryStep = queryStep;

            var result = _prometheusClient.ExecutePromQLQuery(queryParams);

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

            var promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
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

            var promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);

            int currCpuIdx = 1;
            foreach (PrometheusSingleResult cpuMetric in promResp.Data.Result)
            {
                double result = double.Parse(cpuMetric.TimestampAndValue[1]);
                cpusUtilizationDTOs.Add(new SingleCpuUtilizationDTO { CpuIdx = currCpuIdx++, UtilizationPercentage = result });
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

            foreach(SingleCpuUtilizationDTO singleCpuUtilization in cpusUtilizationDTOs )
            {
                singleCpuUtilization.UtilizationPercentage = singleCpuUtilization.UtilizationPercentage / totalCpuCapacity * 100;
                singleCpuUtilization.UtilizationPercentage = Math.Round(singleCpuUtilization.UtilizationPercentage, 3);
                totalCpuUsage += singleCpuUtilization.UtilizationPercentage;
            }

            cpusUtilizationDTOs.Add(new SingleCpuUtilizationDTO()
            {
                CpuIdx = cpusUtilizationDTOs.Count + 1,
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

            var promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
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

            var promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
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
                InstanceAddr = instance
            };

            var result = await _prometheusClient.ExecutePromQLQuery(queryParams);
            //return result.Data.Result[0].TimestampAndValue[0];
            return 0;
        }

        public async Task<double> GetTotalDiskSizeInGB(string instance)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                QueryType = PrometheusQueryType.GetTotalDiskSizeInGB
            };

            PrometheusResponse promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);
            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetTotalMemorySizeInGB(string instance)
        {
            PrometheusQueryParams queryParams = new PrometheusQueryParams()
            {
                InstanceAddr = instance,
                QueryType = PrometheusQueryType.GetTotalMemorySizeInGB
            };

            PrometheusResponse promResp = await _prometheusClient.ExecutePromQLQuery(queryParams);
            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }
    }
}
