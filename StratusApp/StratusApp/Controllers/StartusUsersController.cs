using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services;

namespace StratusApp.Controllers
{
    public class StartusUsersController : Controller
    {
        private readonly IStratusService _stratusService;
        //private readonly CloudApiClient _cloudApiClient;

        public StartusUsersController(IStratusService stratusService) 
        {
            _stratusService = stratusService;
        }

        [HttpGet("GetUserByUsername")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetStratusUser(string username)
        {
            var getStratusUserResp = await _stratusService.GetStratusUser(username);

            if  (getStratusUserResp != null)
            {
                return Ok(getStratusUserResp);
            }
            else
            {
                return BadRequest(getStratusUserResp);
            }
        }

        [HttpGet("GetAllStratusUsers")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetAllStratusUsers()
        {
            var getllStratusUsersResp = await _stratusService.GetAllStratusUsers();

            return Ok(getllStratusUsersResp);
           
        }

        [HttpGet("GetAllAwsPackages")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetAllAwsPackages()
        {
            // var getllStratusUsersResp = await _cloudApiClient.GetAllAwsPackages();

            // return Ok(getllStratusUsersResp);
            return Ok();
        }



    }
}
