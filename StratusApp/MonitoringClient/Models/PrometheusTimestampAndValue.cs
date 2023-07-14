using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MonitoringClient.Models
{
    public class PrometheusTimestampAndValue
    {
        [JsonProperty("")]
        public double? UnixTimeStamp { get; set; }

        [JsonProperty()]
        public string? Value { get; set; }

        //public Dictionary<double, string?>? TSValue{ get; set; }
    }
}