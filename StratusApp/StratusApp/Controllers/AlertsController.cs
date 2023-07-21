using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using MonitoringClient;
using Utils.DTO;
using StratusApp.Services;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class AlertsController : Controller
    {
        private readonly AlertsService _alertsService;

        public AlertsController(AlertsService alertsService)
        {
            _alertsService = alertsService;
        }

        [HttpGet("GetAlerts")]
        public async Task<ActionResult<StratusResponse<List<AlertData>>>> GetInstanceCpuUsage()
        {
            var alertResponse = new StratusResponse<List<AlertData>>();

            alertResponse.Data = _alertsService.GetAlertsCollection().Result;

            return Ok(alertResponse);
        }
    }
}
