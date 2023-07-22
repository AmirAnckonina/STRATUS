using DnsClient;
using MonitoringClient.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils.DTO;
using Utils.Enums;

namespace MonitoringClient
{
    public class PrometheusRequestUtils
    {

        private static Dictionary<string, string> _queryTypeMapper;
       
        public PrometheusRequestUtils()
        {
            _queryTypeMapper = new Dictionary<string, string>();
            SetQueryMapper();
        }  

        private static void SetQueryMapper()
        {
            /*_queryTypeMapper.Add("CpuUsageData", "query=node_cpu_seconds_total{instance=\"{instance}\"}");
            _queryTypeMapper.Add("CpuUsageData", "node_cpu_seconds_total{}");*/
        }

        public Uri CreateEndPointRequestUri(string baseurl, string apipath, string query)
        {
            UriBuilder uriBuilder = new UriBuilder(baseurl);

            uriBuilder.Path = apipath;
            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }

        public string ConcateInstanceAddrWithPort(string instanceAddr)
        {
            return $"{instanceAddr}:9100";
        }

        public string ParseTimePeriodToPrometheusTimeFilterFormat(QueryOverTimePeriod timePeriod)
        {
            string timeFilter;

            switch (timePeriod)
            {
                case QueryOverTimePeriod.Year:
                    timeFilter = "1y";
                    break;

                case QueryOverTimePeriod.Day:
                    timeFilter = "1d";
                    break;

                case QueryOverTimePeriod.Month:
                default:
                    timeFilter = "30d";
                    break;
            }

            return timeFilter; 
        }

        public PrometheusQueryType ParseQueryTypeStrToQueryTypeEnum(string queryTypeStr)
        {
            PrometheusQueryType queryType = 
                (PrometheusQueryType)Enum.Parse(typeof(PrometheusQueryType), queryTypeStr);

            return queryType;
        }

        public string GetExperssionQueryString(PrometheusExpressionQueryType expQueryType)
        {
            string expQueryTypeStr;

            switch (expQueryType)
            {
                case PrometheusExpressionQueryType.RangeQuery:
                    expQueryTypeStr = $"query_range";
                    break;

                case PrometheusExpressionQueryType.InstantQuery:
                    expQueryTypeStr = $"query";
                    break;

                default:
                    throw new Exception("Unknown expQueryType");
            }

            return expQueryTypeStr; 
        }

        public string ParseDateTimeToPromQLDateTimeStrFormat(DateTime dateTime)
        {
            string formattedDateTime = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            return formattedDateTime;
        }

        public string BuildPromQLQueryContentByParams(PrometheusQueryParams queryParams)
        {
            string fullQueryString = string.Empty;
            string queryContent;
            string queryTimeRange;
            string queryStep;

            queryContent = BuildQueryContentByRequestedQueryType(queryParams);
            fullQueryString += queryContent;
            if (queryParams.ExpressionQuery == PrometheusExpressionQueryType.RangeQuery)
            {
                queryTimeRange = BuildQueryTimeRange(queryParams.StartTime, queryParams.EndTime);
                queryStep = BuildQueryStepRange(queryParams.QueryStep);
                fullQueryString += (queryTimeRange + queryStep);
            }

            return fullQueryString;
        }

        private string BuildQueryContentByRequestedQueryType(PrometheusQueryParams queryParams)
        {
            string queryContent = string.Empty;

            switch (queryParams.QueryType)
            {
                case PrometheusQueryType.GetAvgCpuUsageUtilizationOverTime:
                    queryContent = BuildAvgCpuUsageUtilizationOverTime(queryParams);
                    break;
            }

            return queryContent;
        }

        private string BuildQueryTimeRange(DateTime startTime, DateTime endTime)
        {

            string startTimeStr = ParseDateTimeToPromQLDateTimeStrFormat(startTime);
            string endTimeStr = ParseDateTimeToPromQLDateTimeStrFormat(endTime);
            string timeRangeString = $"&start={startTimeStr}&end={endTimeStr}"; //&step=15s"

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

        public string BuildAvgCpuUsageUtilizationOverTime(PrometheusQueryParams queryParams)
        {
            // http://localhost:9090/api/v1/query_range?query=(avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)&start=2023-07-19T20:10:30.781Z&end=2023-07-20T20:11:00.781Z&step=15s
            
            string instanceAddrWithPort = ConcateInstanceAddrWithPort(queryParams.InstanceAddr);
            string timeFilterStr = ParseTimePeriodToPrometheusTimeFilterFormat(queryParams.OverTimeFilter);

            //string query = "query=100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilter}])) * 100)";
            string queryContent = "query=100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilterStr}])) * 100)";

            //string queryContent = "query=(avg_over_time(node_filesystem_free_bytes{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilterStr}])) * 100)";
            
            return queryContent;
        }
    }

}
