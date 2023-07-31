using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    public class PrometheusTimestampAndValue
    {
        [JsonProperty(Order = 0)]
        public double? UnixTimeStamp { get; set; }

        [JsonProperty(Order = 1)]
        public string? Value { get; set; }
    }
}