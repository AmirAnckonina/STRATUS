//using Prometheus;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace MonitoringClient
{
    public class PrometheusClient
    {
        private const string PROM_QUERY_PATH =      "/api/v1/query";
        private const string PROM_BASE_URL =        "http://localhost:9090/";


        private string _basePromUrl;
        private HttpClient _promHttpClient;
        private PrometheusClientUtils _utils;


        public PrometheusClient()
        {
            _utils = new PrometheusClientUtils();
            _promHttpClient = new HttpClient();
        }

        public async Task<string> GetCpuUsage()
        {
            string query = "query=node_cpu_seconds_total{instance=\"18.117.113.181:9100\"}";
            Uri endPointWithQuery =
                _utils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);

            HttpResponseMessage getCpuUsageResponse = await _promHttpClient.GetAsync(endPointWithQuery); 


            // Should decide the return type.
            return await getCpuUsageResponse.Content.ReadAsStringAsync();
        }

        /*public async Task<string> GetTotalDiskSize()
        {

        }

        public async Task<string> GetAvgFreeDiskSize()
        {

        }

        public async Task<string> GetAvgFreeMemorySize()
        {

        }

        public async Task<string> GetAvgFreeDiskSize()
        {

        }*/






    }
}