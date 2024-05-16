using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger) {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChatBot()
        {
            return View();
        }

        public IActionResult MachineLearningModels()
        {
            return View();
        }
    }
}