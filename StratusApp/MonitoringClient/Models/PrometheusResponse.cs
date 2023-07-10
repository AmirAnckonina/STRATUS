using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient.Models
{
    public class PrometheusResponse
    {
        public string Status { get; set; }

        public PrometheusResponseData? Data { get; set; }
    }
}
