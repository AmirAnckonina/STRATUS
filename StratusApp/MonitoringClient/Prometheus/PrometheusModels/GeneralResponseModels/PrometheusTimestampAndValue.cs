using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    public class PrometheusTimestampAndValue
    {
        [JsonProperty(Order = 1)]
        public double? UnixTimeStamp { get; set; }

        [JsonProperty(Order = 2)]
        public string? Value { get; set; }
    }
}