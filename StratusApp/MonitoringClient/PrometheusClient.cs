//using Prometheus;
using System.Net.Http;
using Prometheus;
using static System.Net.WebRequestMethods;

namespace MonitoringClient
{
    public class PrometheusClient
    {
        private HttpClient _promHttpClient;

        private MetricServer _metricServer;

        public PrometheusClient()
        {
            string node_exporter_url = $"http://172.31.40.237:9100/";
            /*_metricServer = new MetricServer(url: node_exporter_url, port: 9100); 
            _metricServer.Start();*/
            _promHttpClient= new HttpClient();
            _promHttpClient.BaseAddress = new Uri(node_exporter_url);
        }

        public async Task<string> GetCpuUsage()
        {
            var response = await _promHttpClient.GetAsync("/metrics");
            return await response.Content.ReadAsStringAsync();
        }   


    }
}