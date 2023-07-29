using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusApi
{
    public class PrometheusSerializer
    {
        public PrometheusSerializer() { }

        public PrometheusResponse DeserializeRawResponse(PrometheusExpressionQueryType expQueryTypr, string respContent)
        {
            return JsonConvert.DeserializeObject<PrometheusResponse>(respContent);
        }
    }
}
