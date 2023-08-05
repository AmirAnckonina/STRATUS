using Amazon.EC2.Model;
using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils.Enums;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    internal class PrometheusQueryBuilder
    {
        private const string QUERY_PREFIX = "query=";

        public PrometheusQueryBuilder()
        {

        }

        public string ConcateInstanceAddrWithPort(string instanceAddr)
        {
            return $"{instanceAddr}:9100";
        }

        private string BuildQueryTimeRange(DateTime startTime, DateTime endTime)
        {

            string startTimeStr = PrometheusQueryParamsUtils.ParseDateTimeToPromQLDateTimeStrFormat(startTime);
            string endTimeStr = PrometheusQueryParamsUtils.ParseDateTimeToPromQLDateTimeStrFormat(endTime);
            string timeRangeString = $"&start={startTimeStr}&end={endTimeStr}";

            return timeRangeString;
        }

        private string BuildQueryStepRange(QueryOverTimePeriod queryStep)
        {
            string queryStepStr = string.Empty;
            int queryStepNumInSec;

            switch (queryStep)
            {
                case QueryOverTimePeriod.Year:
                    queryStepNumInSec = 31557600;
                    break;

                case QueryOverTimePeriod.Day:
                    queryStepNumInSec = 86400;
                    break;

                case QueryOverTimePeriod.Month:
                default:
                    queryStepNumInSec = 2629800;
                    break;
            }

            queryStepStr = $"&step={queryStepNumInSec}s";

            return queryStepStr;
        }

        public string BuildPromQLQueryContentByParams(PrometheusQueryParams queryParams)
        {
            string fullQueryString = string.Empty;
            string queryContent;
            string queryTimeRange;
            string queryStep;

            queryParams.InstanceAddrWithPort = ConcateInstanceAddrWithPort(queryParams.InstanceAddr);
            queryParams.OverTimeFilterStr = PrometheusQueryParamsUtils.ParseTimePeriodToPrometheusTimeFilterFormat(queryParams.OverTimeFilter);
            queryContent = BuildQueryContentByRequestedQueryType(queryParams);
            fullQueryString += queryContent;
            if (queryParams.ExpressionQuery == PrometheusExpressionQueryType.RangeQuery)
            {
                queryTimeRange = BuildQueryTimeRange(queryParams.StartTime, queryParams.EndTime);
                queryStep = BuildQueryStepRange(queryParams.QueryStep);
                fullQueryString += queryTimeRange + queryStep;
            }

            return fullQueryString;
        }

        private string BuildQueryContentByRequestedQueryType(PrometheusQueryParams queryParams)
        {
            string queryContent = QUERY_PREFIX;

            switch (queryParams.QueryType)
            {
                case PrometheusQueryType.GetAvgCpuUsageUtilizationOverTime:
                    queryContent += BuildAvgCpuUsageUtilizationOverTime(queryParams);
                    break;

                case PrometheusQueryType.GetAvgCpuUsageUtilization:
                    queryContent += BuildAvgCpuUsageUtilization(queryParams);
                    break;

                case PrometheusQueryType.GetMaxCpuUsageUtilization:
                    queryContent += BuildMaxCpuUsageUtiliziation(queryParams);
                    break;

                case PrometheusQueryType.GetAvgCpuUtilizationByCpu:
                    queryContent += BuildAvgCpuUtilizationByCpu(queryParams);
                    break;

                case PrometheusQueryType.GetTotalDiskSizeInGB:
                    queryContent += BuildTotalDiskSizeInGB(queryParams);
                    break;

                case PrometheusQueryType.GetAvgFreeDiskSpaceInGB:
                    queryContent += BuildAvgFreeDiskSpaceInGB(queryParams);
                    break;

                case PrometheusQueryType.GetTotalMemorySizeInGB:
                    queryContent += BuildTotalMemorySizeInGB(queryParams);
                    break;

                case PrometheusQueryType.GetAvgFreeMemorySizeInGB:
                    queryContent += BuildAvgFreeMemorySizeInGB(queryParams);
                    break;

                case PrometheusQueryType.GetNumberOfvCPU:
                    queryContent += BuildNumberOfvCPU(queryParams);
                    break;

                case PrometheusQueryType.GetAvgDiskSpaceUsageInGB:
                    queryContent += BuildAvgDiskSpaceUsageInGB(queryParams);
                    break;

                case PrometheusQueryType.GetAvgMemorySizeUsageInGB:
                    queryContent += BuildAvgMemorySizeUsageInGB(queryParams);
                    break;

                default:
                    throw new PrometheusUnsupportedQueryTypeException("Unsupported query type by prometheus.");

            }

            return queryContent;
        }

        private string BuildMaxCpuUsageUtiliziation(PrometheusQueryParams queryParams)
        {
            // 100 - (min(rate(node_cpu_seconds_total{instance='34.125.220.240:9100',mode="idle"}[13d])) * 100)
            return "100 - (min(rate(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mode='idle'}" + $"[{queryParams.OverTimeFilterStr}])) * 100)";
        }          

        private string BuildAvgFreeMemorySizeInGB(PrometheusQueryParams queryParams)
        {
            // avg_over_time(node_memory_MemFree_bytes{instance='34.125.220.240:9100'}[30d])/(1024^3)
            return "avg_over_time(node_memory_MemFree_bytes{instance='" + $"{queryParams.InstanceAddrWithPort}" + "'}" + $"[{queryParams.OverTimeFilterStr}])/(1024^3)";
        }

        private string BuildTotalMemorySizeInGB(PrometheusQueryParams queryParams)
        {
            // node_memory_MemTotal_bytes{instance='34.125.220.240:9100'}/(1024^3)
            return "(node_memory_MemTotal_bytes{instance='" + $"{queryParams.InstanceAddrWithPort}" + "'})/(1024^3)";
        }

        private string BuildAvgMemorySizeUsageInGB(PrometheusQueryParams queryParams)
        {
            // node_memory_MemTotal_bytes{instance='34.125.220.240:9100'}/(1024^3) - avg_over_time(node_memory_MemFree_bytes{instance='34.125.220.240:9100'}[30d])/(1024^3)
            return BuildTotalMemorySizeInGB(queryParams) + " - " + BuildAvgFreeMemorySizeInGB(queryParams);
        }

        private string BuildAvgFreeDiskSpaceInGB(PrometheusQueryParams queryParams)
        {
            // (avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)
            return "avg_over_time(node_filesystem_free_bytes{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mountpoint='/'}" + $"[{queryParams.OverTimeFilterStr}])/(1024^3)";
        }

        private string BuildTotalDiskSizeInGB(PrometheusQueryParams queryParams)
        {
            //sum(node_filesystem_size_bytes{instance='34.125.220.240:9100'})/(1024^3)
            //return "query=sum(node_filesystem_size_bytes{instance='" + $"{queryParams.InstanceAddrWithPort}" + "'})/(1024^3)";
            return "node_filesystem_size_bytes{instance='" + $"{queryParams.InstanceAddrWithPort}" + "', mountpoint='/'}/(1024^3)";
        }

        private string BuildAvgDiskSpaceUsageInGB(PrometheusQueryParams queryParams)
        {
            // (node_filesystem_size_bytes{instance='34.125.220.240:9100', mountpoint='/'}/(1024^3) ) - (node_filesystem_free_bytes{instance='34.125.220.240:9100', mountpoint='/'}/(1024^3) )
            return BuildTotalDiskSizeInGB(queryParams) + " - " + BuildAvgFreeDiskSpaceInGB(queryParams);
        }

        private string BuildAvgCpuUtilizationByCpu(PrometheusQueryParams queryParams)
        {
            return "100 - (avg by (cpu) (rate(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mode='idle'}" + $"[{queryParams.OverTimeFilterStr}])) * 100)";
        }

        private string BuildAvgCpuUsageUtilization(PrometheusQueryParams queryParams)
        {
            // 100 - (avg(rate(node_cpu_seconds_total{instance='34.125.220.240:9100',mode="idle"}[15m])) * 100)
            // 100 * (avg by (instance) (rate(node_cpu_seconds_total{mode!="idle"}[14d])))
            return "100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mode='idle'}" + $"[{queryParams.OverTimeFilterStr}])) * 100)";
        }

        private string BuildNumberOfvCPU(PrometheusQueryParams queryParams)
        {
            // count(node_cpu_seconds_total{instance="34.125.220.240:9100"}) by (cpu)
            return "count(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "'}) by (cpu)";
        }

        public string BuildAvgCpuUsageUtilizationOverTime(PrometheusQueryParams queryParams)
        {
            // http://localhost:9090/api/v1/query_range?query=(avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)&start=2023-07-19T20:10:30.781Z&end=2023-07-20T20:11:00.781Z&step=15s

            //string query = "query=100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilter}])) * 100)";
            string queryContent =
                "100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{queryParams.InstanceAddrWithPort}" + "',mode='idle'}" + $"[{queryParams.OverTimeFilterStr}])) * 100)";

            return queryContent;
        }
    }
}
