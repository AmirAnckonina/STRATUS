using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    public class PrometheusTimestampAndValue
    {
        [JsonProperty("")]
        public double? UnixTimeStamp { get; set; }

        [JsonProperty()]
        public string? Value { get; set; }
    }
}