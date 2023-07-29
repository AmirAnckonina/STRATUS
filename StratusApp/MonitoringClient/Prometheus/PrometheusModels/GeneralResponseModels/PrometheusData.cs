using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels
{
    public class PrometheusData
    {
        [JsonProperty("resultType")]
        public string ResultType { get; set; }

        [JsonProperty("result")]
        public List<PrometheusResult>? Result { get; set; }
        //public List<PrometheusSingleResult>? Result { get; set; }
    }
}
