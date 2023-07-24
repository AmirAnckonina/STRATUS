using Microsoft.AspNetCore.Mvc;
using AwsClient = CloudApiClient.CloudApiClient;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Cors;
using CloudApiClient.AwsServices.AwsUtils;
using StratusApp.Data;
using Utils.DTO;
using StratusApp.Services;

namespace StratusApp.Controllers
{
    //[EnableCors()]
    [EnableCors("AllowAnyOrigin")]
    public class AwsController : Controller
    {
        private readonly AwsService _awsService;
        
        public AwsController(AwsService awsService) 
        {
            _awsService = awsService;
        }

        [HttpGet("GetUserInstanceData")]
        public async Task<ActionResult<StratusResponse<List<InstanceDetails>>>> GetUserInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<InstanceDetails>>();

            userInstanceDataStartusResp.Data = await _awsService.GetInstanceFormalData();

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetInstanceCPUStatistics")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetInstanceCPUStatistics(string instanceId)
        {
            var userInstanceDataStartusResp = new StratusResponse<List<Datapoint>>();

            userInstanceDataStartusResp.Data = await _awsService.GetInstanceCPUStatistics(instanceId);

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetUserInstanceCpuUsageDataOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetUserAwsInstanceCpuUsageDataOverTime(string instanceId, string filterTime = "Month")
        {
            var userInstanceCpuUsageDataOverTimeStartusResp = new StratusResponse<List<CpuUsageData>>();

            userInstanceCpuUsageDataOverTimeStartusResp.Data = await _awsService.GetInstanceCpuUsageOverTime(instanceId, filterTime);

            return Ok(userInstanceCpuUsageDataOverTimeStartusResp);
        }

        [HttpGet("GetInstanceFromAWS")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetInstanceFromAWS()
        {
            var userInstanceStartusResp = new StratusResponse<StratusUser>();

            userInstanceStartusResp.Data = await _awsService.GetInstances();

            return Ok(userInstanceStartusResp);
        }
        
        [HttpGet("GetMoreFittedInstancesFromAWS")]
        public async Task<ActionResult<List<Instance>>> GetMoreFittedInstancesFromAWS(string instanceId)
        {
            var instancesListResponse = new StratusResponse<List<InstanceDetails>>();
            instancesListResponse.Data = await _awsService.GetMoreFittedInstances(instanceId);

            return Ok(instancesListResponse);
        }

        [HttpGet("GetInstanceVolumes")]
        public async Task<ActionResult<StratusResponse<List<Volume>>>> GetInstanceVolumes(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<List<Volume>>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceVolumes(instanceId);

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceTotalVolumesSize")]
        public async Task<ActionResult<StratusResponse<int>>> GetInstanceTotalVolumesSize(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<int>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceTotalVolumesSize(instanceId);

            return Ok(instanceVolumeResponse);
        }
        [HttpGet("GetCurrentInstanceVolumesUsage")]
        public async Task<ActionResult<StratusResponse<double>>> GetCurrentInstanceVolumesUsage(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<double>();

            instanceVolumeResponse.Data = await _awsService.GetCurrentInstanceVolumesUsage(instanceId);

            return Ok(instanceVolumeResponse);
        }
        [HttpGet("GetInstanceOperatingSystem")]
        public async Task<ActionResult<StratusResponse<string>>> GetInstanceOperatingSystem(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<string>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceOperatingSystem(instanceId);

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceBasicDetails")]
        public async Task<ActionResult<InstanceDetails>> GetInstanceBasicDetails(string instanceId)
        {
            var instanceBasicDetailsResponse = new StratusResponse<InstanceDetails>();

            instanceBasicDetailsResponse.Data = await _awsService.GetInstanceBasicDetails(instanceId);

            return Ok(instanceBasicDetailsResponse);
        }
        [HttpGet("getAlternativeMachinesWithScraper")]
        public async Task<ActionResult<StratusResponse<List<AlternativeInstance>>>> getAlternativeMachinesWithScraper()
        {
            var instanceBasicDetailsResponse = new StratusResponse<List<AlternativeInstance>>();

            instanceBasicDetailsResponse.Data = await _awsService.ScrapeInstancesDetails();

            return Ok(instanceBasicDetailsResponse);
        }
    }
}
