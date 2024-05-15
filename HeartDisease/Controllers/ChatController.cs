using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HeartDisease.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {

        private readonly ILogger<ChatController> _logger;
        private readonly HttpClient _httpClient;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
        {
            _logger.LogInformation("Received message: {Message}", message.Message);

            var jsonContent = JsonConvert.SerializeObject(new { message = message.Message });
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending message to Python API");

            try
            {
                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/genai_chat", contentString);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received response from Python API: {Response}", responseString);
                    var jsonResponse = JsonConvert.DeserializeObject<ChatResponse>(responseString);
                    return Json(jsonResponse);
                }
                else
                {
                    _logger.LogError("Error response from Python API: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return Json(new ChatResponse { Response = "An error occurred while processing your message." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending message to the Python API.");
                return Json(new ChatResponse { Response = "An error occurred while sending message." });
            }
        }
    }

    public class ChatMessage
    {
        public string Message { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; }
    }
}
