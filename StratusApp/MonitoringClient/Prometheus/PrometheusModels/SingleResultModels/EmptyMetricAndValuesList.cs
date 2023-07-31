using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
using MonitoringClient.Prometheus.PrometheusModels.MetricModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.SingleResultModels
{
    public class EmptyMetricAndValuesList //: BasePrometheusResult
    {
        [JsonProperty("metric")]
        public EmptyMetric? Metric { get; set; }

        [JsonProperty("values")]
        public List<List<string>>? TimestampsAndValues { get; set; }
    }
}
