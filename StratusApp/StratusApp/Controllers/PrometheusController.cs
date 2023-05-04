using Amazon.CloudWatch.Model;
using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;

namespace StratusApp.Controllers
{
    public class PrometheusController : Controller
    {
        private readonly MonitoringClient.PrometheusClient _prometheusClient;

        public PrometheusController()
        {
            _prometheusClient = new MonitoringClient.PrometheusClient(); 
        }

        [HttpGet("GetCPUUsage")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetUserAwsInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<string>();

            userInstanceDataStartusResp.Data = await _prometheusClient.GetCpuUsage();

            return Ok(userInstanceDataStartusResp);
        }
    }
}
