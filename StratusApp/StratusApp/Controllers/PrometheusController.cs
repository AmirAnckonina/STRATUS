using Amazon.CloudWatch.Model;
using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;
using System.Reflection;

namespace StratusApp.Controllers
{
    public class PrometheusController : Controller
    {
        private readonly MonitoringClient.PrometheusClient _prometheusClient;

        public PrometheusController()
        {
            _prometheusClient = new MonitoringClient.PrometheusClient(); 
        }

        [HttpGet("GetInstanceCpuUsage")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetInstanceCpuUsage(string instance, string timeFilter = "4w")
        {
            var cpuUsageResponse = new StratusResponse<string>();

            cpuUsageResponse.Data = await _prometheusClient.GetCpuUsage(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetTotalDiskSizeInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetTotalDiskSizeInGB(string instance)
        {
            var diskSizeResponse = new StratusResponse<string>();

            diskSizeResponse.Data = await _prometheusClient.GetTotalDiskSizeInGB(instance);

            return Ok(diskSizeResponse);
        }

        [HttpGet("GetAvgAvailableDiskSpaceInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetAvgAvailableDiskSpaceInGB(string instance, string timeFilter = "4w")
        {
            var avgAvailableDiskSpaceResponse = new StratusResponse<string>();

            avgAvailableDiskSpaceResponse.Data = await _prometheusClient.GetAvgAvailableDiskSpaceInGB(instance, timeFilter);

            return Ok(avgAvailableDiskSpaceResponse);
        }

        [HttpGet("GetTotalMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetTotalMemorySizeInGB(string instance)
        {
            var totalMemorySizeResponse = new StratusResponse<string>();

            totalMemorySizeResponse.Data = await _prometheusClient.GetTotalMemorySizeInGB(instance);

            return Ok(totalMemorySizeResponse);
        }

        [HttpGet("GetAvgFreeMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetAvgFreeMemorySizeInGB(string instance, string timeFilter ="4w")
        {
            var freeMemorySizeResponse = new StratusResponse<string>();

            freeMemorySizeResponse.Data = await _prometheusClient.GetAvgFreeMemorySizeInGB(instance, timeFilter);

            return Ok(freeMemorySizeResponse);
        }


    }
}
