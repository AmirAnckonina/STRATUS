using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.Exceptions
{
    internal class PrometheusUnsupportedQueryTypeException : Exception
    {
        public PrometheusUnsupportedQueryTypeException(string exMessage) : base(exMessage) { }
    }
}
