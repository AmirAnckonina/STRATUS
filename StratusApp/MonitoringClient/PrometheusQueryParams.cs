using DnsClient;
using MonitoringClient;
using MonitoringClient.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Enums;

namespace Utils.DTO
{
    public class PrometheusQueryParams
    {
        // public PrometheusQueryParams() { }

        public PrometheusExpressionQueryType ExpressionQuery { get; set; }

        public PrometheusQueryType QueryType { get; set; }

        public string InstanceAddr { get; set; }

        public string InstanceAddrWithPort { get; set; }

        public QueryOverTimePeriod OverTimeFilter { get; set; }   

        public DateTime StartTime { get; set; } 

        public DateTime EndTime { get; set; } 

        public QueryOverTimePeriod QueryStep { get; set; }
    }
}
