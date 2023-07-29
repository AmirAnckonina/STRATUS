using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    public class PrometheusResult
    {
        [JsonProperty("metric"), Newtonsoft.Json.JsonIgnore]
        public object? Metric { get; set; }

        /*[JsonProperty("value")]
        public T2? TimestampAndValue { get; set; }

        [JsonProperty("values")]
        private T2? TimestampAndValueV2 { set { TimestampAndValue = value; } }*/

        [JsonProperty("value")]
        public List<string>? TimestampAndValue { get; set; }

        [JsonProperty("values")]
        private List<string>? TimestampAndValueV2 { set { TimestampAndValue = value; } }
    }
}