using Microsoft.AspNetCore.Mvc;
using AwsClient = CloudApiClient.CloudApiClient;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Cors;
using CloudApiClient.DTO;

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

        /*[HttpGet("GetAllAwsPackages")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetAllAwsPackages()
        {

        }*/

        [HttpGet("GetUserInstanceData")]
        public async Task<ActionResult<StratusResponse<List<VirtualMachineBasicData>>>> GetUserAwsInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<VirtualMachineBasicData>>();

            userInstanceDataStartusResp.Data = await _awsClient.GetInstanceFormalData();
            
            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetInstanceCPUStatistics")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetInstanceCPUStatistics()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<Datapoint>>();

            userInstanceDataStartusResp.Data = await _awsClient.GetInstanceCPUStatistics();

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetUserInstanceCpuUsageDataOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetUserAwsInstanceCpuUsageDataOverTime()
        {
            var userInstanceCpuUsageDataOverTimeStartusResp = new StratusResponse<List<CpuUsageData>>();

            userInstanceCpuUsageDataOverTimeStartusResp.Data = await _awsClient.GetInstanceCpuUsageOverTime();

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
            var instancesListResponse = new StratusResponse<List<VirtualMachineBasicData>>();
            instancesListResponse.Data = await _awsClient.GetMoreFittedInstances(instanceId);

            return Ok(instancesListResponse);
        }
    }
}
