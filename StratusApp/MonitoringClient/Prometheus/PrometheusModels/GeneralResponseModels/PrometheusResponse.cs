using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    
    public class PrometheusResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public PrometheusData? Data { get; set; }
    }
}
