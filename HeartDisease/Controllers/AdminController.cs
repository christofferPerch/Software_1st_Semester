using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Sources()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }

            return RedirectToAction("Sources");
        }

    }
}
