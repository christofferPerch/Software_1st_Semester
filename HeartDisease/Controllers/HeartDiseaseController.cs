﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers {
    [Authorize]
    public class HeartDiseaseController : Controller {
        private readonly ILogger<HeartDiseaseController> _logger;

        public HeartDiseaseController(ILogger<HeartDiseaseController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }
    }
}
