using MonitoringClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;


namespace MonitoringClient
{
    public class PrometheusClient
    {
        private const string PROM_QUERY_PATH =      "/api/v1/query";
        private const string PROM_BASE_URL =        "http://localhost:9090/";
        //Test

        private HttpClient _promHttpClient;
        private PrometheusRequestUtils _requestsUtils;
        private PrometheusResponseUtils _responseUtils;
        
        public PrometheusClient()
        {
            _requestsUtils = new PrometheusRequestUtils();
            _responseUtils = new PrometheusResponseUtils();
            _promHttpClient = new HttpClient();
        }

        public async Task<string> GetNumberOfvCPU(string instanceAddr)
        {
            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // count(node_cpu_seconds_total{instance="34.125.220.240:9100"}) by (cpu)
            string query = "query=count(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "'}) by (cpu)";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getNoOfvCPU = await _promHttpClient.GetAsync(endPointWithQuery);
            return await getNoOfvCPU.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCpuUsage(string instanceAddr, string timeFilter)
        {
            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=(avg by(instance) (rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "'}" + $"[{timeFilter}]) * (100)))";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getCpuUsageResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            // Should decide the return type.
            return await getCpuUsageResponse.Content.ReadAsStringAsync();
        }

        public async Task<string> GetTotalDiskSizeInGB(string instanceAddr)
        {
            PrometheusResponse promResp = new PrometheusResponse();

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            //sum(node_filesystem_size_bytes{instance='34.125.220.240:9100'})/(1024^3)
            string query = "query=sum(node_filesystem_size_bytes{instance='" + $"{instanceAddrWithPort}" + "'})/(1024^3)";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getDiskSizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);
            string respContent = await getDiskSizeResponse.Content.ReadAsStringAsync();
            var promRespObj = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            if (promRespObj != null)
            {
                return promRespObj.Data.Result.FirstOrDefault().TimestampAndValue[1]; // TSValue[1];// MetricAndValue.TimestampAndValue.Value;
            }
            else return string.Empty;
            // Should decide the return type.s
        }

        public async Task<string> GetAvgAvailableDiskSpaceInGB(string instanceAddr, string timeFilter)
        {
            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // (avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)
            string query = 
                "query=avg_over_time(node_filesystem_free_bytes{instance='" +
                $"{instanceAddrWithPort}" + "',mountpoint='/'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getAvgAvailableDiskSpaceResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            // Should decide the return type.
            return await getAvgAvailableDiskSpaceResponse.Content.ReadAsStringAsync();
        }

        
        public async Task<string> GetTotalMemorySizeInGB(string instanceAddr)
        {
            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=(node_memory_MemTotal_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'})/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getTotalMemorySizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            // Should decide the return type.
            return await getTotalMemorySizeResponse.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAvgFreeMemorySizeInGB(string instanceAddr, string timeFilter)
        {
            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=avg_over_time(node_memory_MemFree_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, PROM_QUERY_PATH, query);
            HttpResponseMessage getAvgFreeMemorySizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            // Should decide the return type.
            return await getAvgFreeMemorySizeResponse.Content.ReadAsStringAsync();
        }
    }
}