using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        
    }

}
