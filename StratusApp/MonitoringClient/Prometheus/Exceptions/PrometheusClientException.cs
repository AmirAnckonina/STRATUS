using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Prometheus.Exceptions
{
    public class PrometheusClientException : Exception
    {
        public PrometheusClientException(string message) 
            : base (message)
        {
        }
    }
}
