using MonitoringClient.Prometheus.PrometheusModels.MetricModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.SingleResultModels
{
    internal class VectorSingleResult
    {
        [JsonProperty("metric"), Newtonsoft.Json.JsonIgnore]
        public PrometheusMetric? Metric { get; set; }

        [JsonProperty("value")]
        public List<string>? TimestampAndValue { get; set; }
    }
}
