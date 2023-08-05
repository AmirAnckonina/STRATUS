using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.Enums
{
    public enum PrometheusConcreteResultType
    {
        EmptyMetricAndSingleValue,
        EmptyMetricAndValuesList,
        ListOfCpuMetricAndSingleValue
    }
}
