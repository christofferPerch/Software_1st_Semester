using Microsoft.AspNetCore.Mvc;

namespace HeartDisease.Controllers
{
    public class ChatBotController : Controller
    {
        public IActionResult Sources()
        {
            return View();
        }

        public IActionResult Train()
        {
            return View();
        }
    }
}
