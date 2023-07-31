using Amazon.EC2.Model;
using Amazon.Runtime;
using MonitoringClient.Prometheus.Enums;
using MonitoringClient.Prometheus.PrometheusModels;
using MonitoringClient.Prometheus.PrometheusModels.GeneralResponseModels;
using MonitoringClient.Prometheus.PrometheusModels.MetricModels;
using MonitoringClient.Prometheus.PrometheusModels.SingleResultModels;
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
        private readonly PrometheusQueryBuilder _queryBuilder;
        private readonly PrometheusSerializer _prometheusSerializer;

        public PrometheusClient()
        {
            _promHttpClient = new HttpClient();
            _queryBuilder = new PrometheusQueryBuilder();
            _prometheusSerializer = new PrometheusSerializer();
        }

        /*public async Task<PrometheusResponse> ExecutePromQLQuery(PrometheusQueryParams queryParams)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(queryParams.ExpressionQuery);

            string queryPath = PROM_QUERY_PATH + expQueryType;

            string query = _queryBuilder.BuildPromQLQueryContentByParams(queryParams);

            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);

            HttpResponseMessage rawResp = await _promHttpClient.GetAsync(endPointWithQuery);

            string respContent = await rawResp.Content.ReadAsStringAsync();

            //PrometheusResponse? promResp = _prometheusSerializer.DeserializeRawResponse(queryParams.ExpressionQuery, respContent);
          
        }*/

        public async Task<T> ExecutePromQLQuery<T>(PrometheusQueryParams queryParams)
        {
            string expQueryType = PrometheusQueryParamsUtils.GetExperssionQueryString(queryParams.ExpressionQuery);
            string queryPath = PROM_QUERY_PATH + expQueryType;
            string query = _queryBuilder.BuildPromQLQueryContentByParams(queryParams);
            Uri endPointWithQuery = HttpRequestUtils.CreateEndPointRequestUri(PROM_BASE_URL, queryPath, query);
            HttpResponseMessage rawResp = await _promHttpClient.GetAsync(endPointWithQuery);
            string respContent = await rawResp.Content.ReadAsStringAsync();
            T result = _prometheusSerializer.DeserializeRawResponse<T>(queryParams.QueryType, respContent);
            return result;
        }
    }
}