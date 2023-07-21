using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;
using StratusApp.Services.Collector;
using Utils.DTO;

namespace StratusApp.Controllers
{
    public class CollectorController : Controller
    {
        private readonly CollectorService _collectorService;

        public CollectorController(CollectorService collectorService) 
        {
            _collectorService = collectorService;
        }

        [HttpGet("GetAvgCpuUsageUtilizationOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetAvgCpuUsageUtilizationOverTime(
            string instance = "34.125.220.240",
            string timeFilter = "month")
        {
            try
            {
                var avgCpuOverTimeResponse = new StratusResponse<List<CpuUsageData>>();

                avgCpuOverTimeResponse.Data = await _collectorService.GetAvgCpuUsageUtilizationOverTime(instance, timeFilter);

                return Ok(avgCpuOverTimeResponse);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
