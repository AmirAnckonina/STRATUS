using MonitoringClient;
using MonitoringClient.Enums;
using System;
using Utils.DTO;
using Utils.Enums;

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

        /*public async Task<List<CpuUsageData>> GetAvgCpuUsageUtilizationOverTime(string instance, string timePeriodStr)
        {
            List<CpuUsageData> cpuUsageDataList = new List<CpuUsageData>();
            int queriesToExec = 0;
            QueryOverTimePeriod overallTimePeriod;
            QueryOverTimePeriod singleQueryTimePeriod;
            QueryTimeOffsetType timeOffsetType;
            int currentOffset;


            overallTimePeriod = _collectorUtils.ParseTimePeriodStrToTimePeriodEnum(timePeriodStr);

            //export to method inCollectorUtils.
            switch (overallTimePeriod)
            {
                case QueryOverTimePeriod.Day:
                    singleQueryTimePeriod = QueryOverTimePeriod.Hour;
                    timeOffsetType = QueryTimeOffsetType.Hour;
                    queriesToExec = 24;
                    break;


                case QueryOverTimePeriod.Year:
                   singleQueryTimePeriod = QueryOverTimePeriod.Month;
                   timeOffsetType = QueryTimeOffsetType.Month;
                   queriesToExec = 12;
                   break;

                case QueryOverTimePeriod.Month:
                case QueryOverTimePeriod.None:
                    singleQueryTimePeriod = QueryOverTimePeriod.Day;
                    timeOffsetType = QueryTimeOffsetType.Day;
                    queriesToExec = 30;
                    break;

                default:
                    throw new Exception("Query overall time isn't supported.");
            }

            for (int i = 0; i < queriesToExec; i++)
            {
                //CpuUsageData singleCpuUtilizationResult = await _prometheusClient.GetAvgCpuUsageUtilization(instance, singleQueryTimePeriod, timeOffsetType, currentOffset);

            }


            return cpuUsageDataList;

        }*/

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
    }
}
