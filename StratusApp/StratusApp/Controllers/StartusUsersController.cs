using DTO;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services;
using StratusApp.Services.AlertsService;
using Utils.DTO;
using Utils.Utils;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class StartusUsersController : Controller
    {
        private readonly IStratusService _stratusService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StartusUsersController(IStratusService stratusService, IHttpContextAccessor httpContextAccessor)
        {
            _stratusService = stratusService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetUserByEmail")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetUserByEmail()
        {
            string email = SessionUtils.GetSessionId(_httpContextAccessor);

            StratusResponse<StratusUser> userResponse = new StratusResponse<StratusUser>();
            try
            {
                userResponse.Data = await _stratusService.GetUserByEmail(email);

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("UpdateUserDetails")]
        public async Task<ActionResult<StratusResponse<string>>> UpdateUserDetails([FromBody] StratusUser user)
        {
            string email = SessionUtils.GetSessionId(_httpContextAccessor);
            bool result = await _stratusService.UpdateUserDetails(email, user);

            if (result)
            {
                var response = new StratusResponse<string>
                {
                    Data = "User Details saved successfully"
                };

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
