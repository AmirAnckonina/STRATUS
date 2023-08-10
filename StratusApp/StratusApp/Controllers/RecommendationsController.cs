using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StratusApp.Models.Responses;
using StratusApp.Services;
using StratusApp.Services.Recommendations;
using Utils.DTO;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class RecommendationsController : Controller
    {
        private readonly RecommendationsService _recommendationsService;

        public RecommendationsController(RecommendationsService recommendationsService)
        {
            _recommendationsService = recommendationsService;
        }
    
        [HttpGet("GetRecommendationsInstances")]
        public async Task<ActionResult<List<CustomInstances>>> GetMoreFittedInstancesFromAWS()
        {
            var instancesListResponse = new StratusResponse<List<CustomInstances>>();
            instancesListResponse.Data = await _recommendationsService.GetRecommendationsInstances();

            return Ok(instancesListResponse);
        }
    }
}
