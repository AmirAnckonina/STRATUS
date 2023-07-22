using Amazon.Runtime;
using MonitoringClient.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;
using Utils.DTO;
using Utils.Enums;
using static System.Net.WebRequestMethods;


namespace MonitoringClient
{
    public class PrometheusClient
    {
        private const string PROM_BASE_URL =        "http://localhost:9090/";
        private const string PROM_QUERY_PATH =      "/api/v1/";

        private HttpClient _promHttpClient;
        private PrometheusRequestUtils _requestsUtils;
        private PrometheusResponseUtils _responseUtils;
        
        public PrometheusClient()
        {
            _requestsUtils = new PrometheusRequestUtils();
            _responseUtils = new PrometheusResponseUtils();
            _promHttpClient = new HttpClient();
        }

        public PrometheusRequestUtils ReqUtils { get; set; }
        public PrometheusResponseUtils RespUtils { get; set; }

        public async Task<string> GetNumberOfvCPU(string instanceAddr)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;


            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // count(node_cpu_seconds_total{instance="34.125.220.240:9100"}) by (cpu)
            string query = "query=count(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "'}) by (cpu)";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getNoOfvCPU = await _promHttpClient.GetAsync(endPointWithQuery);
            return await getNoOfvCPU.Content.ReadAsStringAsync();
        }

        public async Task<double> GetAvgCpuUsageUtilization(string instanceAddr, string timeFilter)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // 100 - (avg(rate(node_cpu_seconds_total{instance='34.125.220.240:9100',mode="idle"}[15m])) * 100)
            // 100 * (avg by (instance) (rate(node_cpu_seconds_total{mode!="idle"}[14d])))
            string query = "query=100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilter}])) * 100)";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getCpuUsageResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getCpuUsageResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<List<SingleCpuUtilizationDTO>> GetAvgCpuUtilizationByCpu(string instanceAddr, string timeFilter)
        {
            List<SingleCpuUtilizationDTO> cpusUtilizationDTOs = new List<SingleCpuUtilizationDTO>();

            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // 100 - (avg by (cpu) (rate(node_cpu_seconds_total{instance='34.125.220.240:9100',mode="idle"}[15m])) * 100)
            string query = "query=100 - (avg by (cpu) (rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilter}])) * 100)";
            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getCpuUsageByCpuResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getCpuUsageByCpuResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            int currCpuIdx = 0;
            foreach(PrometheusMetricAndWrappedValue cpuMetric in promResp.Data.Result)
            {
                double result = double.Parse(cpuMetric.TimestampAndValue[1]);
                cpusUtilizationDTOs.Add(new SingleCpuUtilizationDTO { CpuIdx = currCpuIdx++, UtilizationPercentage = result});
            }

            return cpusUtilizationDTOs;
        }

        public async Task<double> GetTotalDiskSizeInGB(string instanceAddr)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);

            //sum(node_filesystem_size_bytes{instance='34.125.220.240:9100'})/(1024^3)
            string query = "query=sum(node_filesystem_size_bytes{instance='" + $"{instanceAddrWithPort}" + "'})/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getDiskSizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);
            string respContent = await getDiskSizeResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result =  double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgFreeDiskSpaceInGB(string instanceAddr, string timeFilter)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            // (avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)
            string query = 
                "query=avg_over_time(node_filesystem_free_bytes{instance='" +
                $"{instanceAddrWithPort}" + "',mountpoint='/'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getAvgAvailableDiskSpaceResponse = await _promHttpClient.GetAsync(endPointWithQuery);
            
            string respContent = await getAvgAvailableDiskSpaceResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetTotalMemorySizeInGB(string instanceAddr)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=(node_memory_MemTotal_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'})/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getTotalMemorySizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getTotalMemorySizeResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgFreeMemorySizeInGB(string instanceAddr, string timeFilter)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _requestsUtils.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=avg_over_time(node_memory_MemFree_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getAvgFreeMemorySizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getAvgFreeMemorySizeResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }      

        public Task<InstanceDetailsDTO> GetInstanceSpecifications(string instanceAddr)
        {
            // TODO: Impl calling to all methods under PromClient (Disk, Memrory, Cpu..)
            throw new NotImplementedException();
        }

        public async Task<PrometheusResponse> ExecutePromQLQuery(PrometheusQueryParams queryParams)
        {
            string expQueryType = _requestsUtils.GetExperssionQueryString(queryParams.ExpressionQuery);

            string queryPath = PROM_QUERY_PATH + expQueryType;

            string query = _requestsUtils.BuildPromQLQueryContentByParams(queryParams);

            Uri endPointWithQuery = _requestsUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
         
            HttpResponseMessage rawResp = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await rawResp.Content.ReadAsStringAsync();

            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            return promResp;
        }
    }
}