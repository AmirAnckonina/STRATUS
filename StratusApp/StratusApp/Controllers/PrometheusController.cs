using Amazon.CloudWatch.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;
using StratusApp.Services;
using System.Linq.Expressions;
using StratusApp.Services.MongoDBServices;
using System.Reflection;
using Utils.DTO;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class PrometheusController : Controller
    {
        private readonly MonitoringClient.PrometheusClient _prometheusClient;
        private readonly IStratusService _stratusService;

        public PrometheusController(IStratusService stratusService)
        {
            _prometheusClient = new MonitoringClient.PrometheusClient();
            _stratusService = stratusService;
        }

        [HttpGet("GetNumberOfvCPU")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetNumberOfvCPU(string instance)
        {
            var cpuUsageResponse = new StratusResponse<string>();

            cpuUsageResponse.Data = await _prometheusClient.GetNumberOfvCPU(instance);

            return Ok(cpuUsageResponse);
        }       

        [HttpGet("GetAvgCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<double>>> GetCpuUsageUtilization(string instance = "34.125.220.240", string timeFilter = "4w")
        {
            var cpuUsageResponse = new StratusResponse<double>();

            cpuUsageResponse.Data = await _prometheusClient.GetAvgCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetMaxCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<double>>> GetMaxCpuUsageUtilization(string instance = "34.125.220.240", string timeFilter = "4w")
        {
            var cpuUsageResponse = new StratusResponse<double>();

            /// Should be modified to get the max and not the avarage!
            /// Dummy dummy dummy
            cpuUsageResponse.Data = await _prometheusClient.GetAvgCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }


        [HttpGet("GetAvgCpuUtilizationByCpu")]
        public async Task<ActionResult<StratusResponse<List<SingleCpuUtilizationDTO>>>> GetAvgCpuUtilizationByCpu(string instance, string timeFilter = "4w")
        {
            try
            {
                var freeMemorySizeResponse = new StratusResponse<List<SingleCpuUtilizationDTO>>();

                freeMemorySizeResponse.Data = await _prometheusClient.GetAvgCpuUtilizationByCpu(instance, timeFilter);

                return Ok(freeMemorySizeResponse);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTotalDiskSizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetTotalDiskSizeInGB(string instance = "34.125.220.240")
        {
            try
            {
                var diskSizeResponse = new StratusResponse<double>();

                diskSizeResponse.Data = await _prometheusClient.GetTotalDiskSizeInGB(instance);

                return Ok(diskSizeResponse);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgFreeDiskSpaceInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgFreeDiskSpaceInGB(string instance, string timeFilter = "4w")
        {
            try
            {
                var avgAvailableDiskSpaceResponse = new StratusResponse<double>();

                avgAvailableDiskSpaceResponse.Data = await _prometheusClient.GetAvgFreeDiskSpaceInGB(instance, timeFilter);

                return Ok(avgAvailableDiskSpaceResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTotalMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetTotalMemorySizeInGB(string instance)
        {
            try
            {

                var totalMemorySizeResponse = new StratusResponse<double>();

                totalMemorySizeResponse.Data = await _prometheusClient.GetTotalMemorySizeInGB(instance);

                return Ok(totalMemorySizeResponse);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        //todo : the timefilter structure from UI is : "month", "year", etc.. we should convert it as well.
        [HttpGet("GetAvgFreeMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgFreeMemorySizeInGB(string instance, string timeFilter ="4w")
        {
            try
            {
                var freeMemorySizeResponse = new StratusResponse<double>();

                freeMemorySizeResponse.Data = await _prometheusClient.GetAvgFreeMemorySizeInGB(instance, timeFilter);

                return Ok(freeMemorySizeResponse);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllUserResourcesDetails")]
        public async Task<ActionResult<StratusResponse<List<InstanceDetailsDTO>>>> GetAllUserResourcesDetails(string userEmail)
        {
            try
            {
                var getAllUserResourcesDetailsResponse = new StratusResponse<List<InstanceDetailsDTO>>();
                List<InstanceDetailsDTO> userInstancesDetails = new List<InstanceDetailsDTO>();

                List<string> userInstacesAddresses = new List<string>();
                //List<string> userInstaces = _dbClient.GetAllUserResourcesDetails(userEmail);

                foreach(string instanceAddr in userInstacesAddresses) 
                {
                    // For InstanceId = "x.x.x.x" we need to build single UserInstanceDetailsDTO and add it to the resultList.
                    InstanceDetailsDTO singleInstanceDetails = await _prometheusClient.GetInstanceSpecifications(instanceAddr);
                    userInstancesDetails.Add(singleInstanceDetails);
                }


                return Ok(getAllUserResourcesDetailsResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
