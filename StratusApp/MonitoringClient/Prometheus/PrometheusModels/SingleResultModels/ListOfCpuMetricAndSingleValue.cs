using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.SingleResultModels
{
    public class ListOfCpuMetricAndSingleValue
    {
        [JsonProperty()]
        public List<CpuMetricAndSingleValue> CpuMetricAndSingleValueList { get; set; }  
    }
}
