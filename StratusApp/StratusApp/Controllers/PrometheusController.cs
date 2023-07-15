using Amazon.CloudWatch.Model;
using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;
using System.Reflection;
using Utils.DTO;

namespace StratusApp.Controllers
{
    public class PrometheusController : Controller
    {
        private readonly MonitoringClient.PrometheusClient _prometheusClient;

        public PrometheusController()
        {
            _prometheusClient = new MonitoringClient.PrometheusClient(); 
        }

        [HttpGet("GetNumberOfvCPU")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetNumberOfvCPU(string instance)
        {
            var cpuUsageResponse = new StratusResponse<string>();

            cpuUsageResponse.Data = await _prometheusClient.GetNumberOfvCPU(instance);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetAlerts")]
        public async Task<ActionResult<StratusResponse<List<AlertData>>>> GetInstanceCpuUsage()
        {
            var alertResponse = new StratusResponse<List<AlertData>>();

            alertResponse.Data = _prometheusClient.GetAlerts();

            return Ok(alertResponse);
        }

        [HttpGet("GetCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetCpuUsageUtilization(string instance, string timeFilter = "4w")
        {
            var cpuUsageResponse = new StratusResponse<string>();

            cpuUsageResponse.Data = await _prometheusClient.GetAvgCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }

       /* [HttpGet("GetCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetAvgCpuUsageUtilization(string instance, string timeFilter = "4w")
        {
            var cpuUsageResponse = new StratusResponse<string>();

            cpuUsageResponse.Data = await _prometheusClient.GetAvgCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }*/

        [HttpGet("GetTotalDiskSizeInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetTotalDiskSizeInGB(string instance = "34.125.220.240")
        {
            var diskSizeResponse = new StratusResponse<string>();

            diskSizeResponse.Data = await _prometheusClient.GetTotalDiskSizeInGB(instance);

            return Ok(diskSizeResponse);
        }

        [HttpGet("GetAvgFreeDiskSpaceInGB")]
        public async Task<ActionResult<StratusResponse<string>>> GetAvgFreeDiskSpaceInGB(string instance, string timeFilter = "4w")
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
