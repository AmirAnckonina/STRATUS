using Microsoft.AspNetCore.Mvc;
using AwsClient = CloudApiClient.CloudApiClient;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Cors;
using CloudApiClient.DTO;
using CloudApiClient.Utils;

namespace StratusApp.Controllers
{
    //[EnableCors()]
    [EnableCors("AllowAnyOrigin")]
    public class AwsController : Controller
    {

        private readonly AwsClient _awsClient;

        public AwsController() 
        {
            _awsClient = new AwsClient();
        }

        [HttpGet("GetUserInstanceData")]
        public async Task<ActionResult<StratusResponse<List<InstanceDetails>>>> GetUserAwsInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<InstanceDetails>>();

            userInstanceDataStartusResp.Data = await _awsClient.GetInstanceFormalData();

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetInstanceCPUStatistics")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetInstanceCPUStatistics()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<Datapoint>>();

            userInstanceDataStartusResp.Data = await _awsClient.GetInstanceCPUStatistics("");

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetUserInstanceCpuUsageDataOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetUserAwsInstanceCpuUsageDataOverTime(string instanceId, string filterTime = "Month")
        {
            var userInstanceCpuUsageDataOverTimeStartusResp = new StratusResponse<List<double>>();

            userInstanceCpuUsageDataOverTimeStartusResp.Data = await _awsClient.GetInstanceCpuUsageOverTime(instanceId, filterTime);

            return Ok(userInstanceCpuUsageDataOverTimeStartusResp);
        }
        [HttpGet("GetInstanceFromAWS")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetInstanceFromAWS()
        {
            var userInstanceStartusResp = new StratusResponse<StratusUser>();

            userInstanceStartusResp.Data = await _awsClient.GetInstances();

            return Ok(userInstanceStartusResp);
        }

        [HttpGet("GetMoreFittedInstancesFromAWS")]
        public async Task<ActionResult<List<Instance>>> GetMoreFittedInstancesFromAWS(string instanceId)
        {
            var instancesListResponse = new StratusResponse<List<InstanceDetails>>();
            instancesListResponse.Data = await _awsClient.GetMoreFittedInstances(instanceId);

            return Ok(instancesListResponse);
        }

        [HttpGet("GetInstanceVolumes")]
        public async Task<ActionResult<StratusResponse<List<Volume>>>> GetInstanceVolumes()
        {
            var instanceVolumeResponse = new StratusResponse<List<Volume>>();

            instanceVolumeResponse.Data = await _awsClient.GetInstanceVolumes();

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceTotalVolumesSize")]
        public async Task<ActionResult<StratusResponse<int>>> GetInstanceTotalVolumesSize()
        {
            var instanceVolumeResponse = new StratusResponse<int>();

            instanceVolumeResponse.Data = await _awsClient.GetInstanceTotalVolumesSize();

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceOperatingSystem")]
        public async Task<ActionResult<StratusResponse<string>>> GetInstanceOperatingSystem()
        {
            var instanceVolumeResponse = new StratusResponse<string>();

            instanceVolumeResponse.Data = await _awsClient.GetInstanceOperatingSystem("");

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceBasicDetails")]
        public async Task<ActionResult<InstanceDetails>> GetInstanceBasicDetails(string instanceId)
        {
            var instanceBasicDetailsResponse = new StratusResponse<InstanceDetails>();

            instanceBasicDetailsResponse.Data = await _awsClient.GetInstanceBasicDetails(instanceId);

            return Ok(instanceBasicDetailsResponse);
        }
    }
}
