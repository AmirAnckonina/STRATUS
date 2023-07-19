using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
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
        public async Task<ActionResult<StratusResponse<List<BsonDocument>>>> GetInstanceCpuUsage()
        {
            var alertResponse = new StratusResponse<List<BsonDocument>>();

            alertResponse.Data = _alertsService.GetAlertsTable();

            return Ok(alertResponse);
        }

    }
}
