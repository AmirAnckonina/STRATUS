using Amazon.EC2.Model;
using Amazon.Runtime;
using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusModels;
using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
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


namespace MonitoringClient.Prometheus.PrometheusApi
{
    public class PrometheusClient
    {
        private const string PROM_BASE_URL = "http://localhost:9090/";
        private const string PROM_QUERY_PATH = "/api/v1/";

        private HttpClient _promHttpClient;
        private PrometheusSerializer _responseUtils;
        private readonly PrometheusQueryBuilder _queryBuilder;
        private readonly PrometheusSerializer _prometheusSerializer;

        public PrometheusClient()
        {
            _responseUtils = new PrometheusSerializer();
            _promHttpClient = new HttpClient();
            _queryBuilder = new PrometheusQueryBuilder();
            _prometheusSerializer = new PrometheusSerializer();
        }

        public async Task<string> GetNumberOfvCPU(string instanceAddr)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;


            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);
            // count(node_cpu_seconds_total{instance="34.125.220.240:9100"}) by (cpu)
            string query = "query=count(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "'}) by (cpu)";
            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getNoOfvCPU = await _promHttpClient.GetAsync(endPointWithQuery);
            return await getNoOfvCPU.Content.ReadAsStringAsync();
        }

        public async Task<double> GetAvgCpuUsageUtilization(string instanceAddr, string timeFilter)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);
            // 100 - (avg(rate(node_cpu_seconds_total{instance='34.125.220.240:9100',mode="idle"}[15m])) * 100)
            // 100 * (avg by (instance) (rate(node_cpu_seconds_total{mode!="idle"}[14d])))
            string query = "query=100 - (avg(rate(node_cpu_seconds_total{instance='" + $"{instanceAddrWithPort}" + "',mode='idle'}" + $"[{timeFilter}])) * 100)";
            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getCpuUsageResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getCpuUsageResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetTotalDiskSizeInGB(string instanceAddr)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);

            //sum(node_filesystem_size_bytes{instance='34.125.220.240:9100'})/(1024^3)
            string query = "query=sum(node_filesystem_size_bytes{instance='" + $"{instanceAddrWithPort}" + "'})/(1024^3)";

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getDiskSizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);
            string respContent = await getDiskSizeResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgFreeDiskSpaceInGB(string instanceAddr, string timeFilter)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);
            // (avg_over_time(node_filesystem_free_bytes{instance='34.125.220.240:9100',mountpoint='/'}[4w]))/(1024^3)
            string query =
                "query=avg_over_time(node_filesystem_free_bytes{instance='" +
                $"{instanceAddrWithPort}" + "',mountpoint='/'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getAvgAvailableDiskSpaceResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getAvgAvailableDiskSpaceResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetTotalMemorySizeInGB(string instanceAddr)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=(node_memory_MemTotal_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'})/(1024^3)";

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage getTotalMemorySizeResponse = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await getTotalMemorySizeResponse.Content.ReadAsStringAsync();
            PrometheusResponse? promResp = JsonConvert.DeserializeObject<PrometheusResponse>(respContent);

            double result = double.Parse(promResp.Data.Result.FirstOrDefault()?.TimestampAndValue[1]);
            return result;
        }

        public async Task<double> GetAvgFreeMemorySizeInGB(string instanceAddr, string timeFilter)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(PrometheusExpressionQueryType.InstantQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;

            string instanceAddrWithPort = _queryBuilder.ConcateInstanceAddrWithPort(instanceAddr);
            string query = "query=avg_over_time(node_memory_MemFree_bytes{instance='" +
                $"{instanceAddrWithPort}" + "'}" + $"[{timeFilter}])/(1024^3)";

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
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
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(queryParams.ExpressionQuery);

            string queryPath = PROM_QUERY_PATH + expQueryType;

            string query = _queryBuilder.BuildPromQLQueryContentByParams(queryParams);

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);

            HttpResponseMessage rawResp = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await rawResp.Content.ReadAsStringAsync();

            PrometheusResponse? promResp = _prometheusSerializer.DeserializeRawResponse(queryParams.ExpressionQuery, respContent);

            return promResp;
        }
    }
}