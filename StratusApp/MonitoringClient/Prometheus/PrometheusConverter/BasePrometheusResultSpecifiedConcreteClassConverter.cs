using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.PrometheusConverter
{
    public class BasePrometheusResultSpecifiedConcreteClassConverter : DefaultContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            if (typeof(BasePrometheusResult).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
}
