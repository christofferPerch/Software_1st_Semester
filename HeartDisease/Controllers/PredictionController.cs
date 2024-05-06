using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers
{
    public class PredictionController : Controller
    {
        private readonly ILogger<PredictionController> _logger;

        public PredictionController(ILogger<PredictionController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
