using Microsoft.AspNetCore.Mvc;

namespace StratusApp.Controllers
{
    public class RecommendationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
