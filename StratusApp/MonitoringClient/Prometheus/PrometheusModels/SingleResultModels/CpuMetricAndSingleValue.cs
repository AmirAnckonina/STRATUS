using MonitoringClient.Prometheus.PrometheusModels.OldPrometheusModels_bkup;
using MonitoringClient.Prometheus.PrometheusModels.MetricModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.SingleResultModels
{
    public class CpuMetricAndSingleValue 
    {
        [JsonProperty("metric")]
        public CpuUtilizationByVCpuMetric? Metric { get; set; }

        [JsonProperty("value")]
        public List<string>? TimestampsAndValues { get; set; }
    }
}
