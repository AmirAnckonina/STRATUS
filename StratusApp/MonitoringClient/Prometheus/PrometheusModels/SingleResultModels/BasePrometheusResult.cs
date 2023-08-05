using MonitoringClient.Prometheus.PrometheusConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusModels.SingleResultModels
{
    [JsonConverter(typeof(BasePrometheusResultConverter))]
    public abstract class BasePrometheusResult
    {
    }
}
