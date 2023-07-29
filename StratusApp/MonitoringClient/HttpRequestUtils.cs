using DnsClient;
using MonitoringClient.Prometheus.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils.DTO;
using Utils.Enums;

namespace MonitoringClient
{
    internal static class HttpRequestUtils
    {
        public static Uri CreateEndPointRequestUri(string baseurl, string apipath, string query)
        {
            UriBuilder uriBuilder = new UriBuilder(baseurl);

            uriBuilder.Path = apipath;
            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }
    }
}
