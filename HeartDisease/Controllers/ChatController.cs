using System.Security.Claims;
using System.Text;
using HeartDisease.Models;
using HeartDisease.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace HeartDisease.Controllers {
    [Authorize]
    public class ChatController : Controller {
        private readonly ILogger<ChatController> _logger;
        private readonly HttpClient _httpClient;
        private readonly ChatBotHistoryService _chatBotHistoryService;

        public ChatController(ILogger<ChatController> logger, ChatBotHistoryService chatBotHistoryService) {
            _logger = logger;
            _chatBotHistoryService = chatBotHistoryService;
            _httpClient = new HttpClient();
        }

        public IActionResult Index() {
            return View();
        }


        #region Send Message ChatBot
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessage message) {
            _logger.LogInformation("Received message: {Message}", message.Message);

            var jsonContent = JsonConvert.SerializeObject(new { message = message.Message });
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending message to Python API");

            try {
                var response = await _httpClient.PostAsync("http://127.0.0.1:5000/genai_chat", contentString);

                if (response.IsSuccessStatusCode) {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received response from Python API: {Response}", responseString);
                    var jsonResponse = JsonConvert.DeserializeObject<ChatResponse>(responseString);

                    var chatBotHistory = new ChatBotHistory {
                        ChatBotId = 1,
                        UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        Message = message.Message,
                        Response = jsonResponse.Response
                    };
                    await _chatBotHistoryService.InsertChatHistoryAsync(chatBotHistory);

                    return Json(jsonResponse);
                } else {
                    _logger.LogError("Error response from Python API: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return Json(new ChatResponse { Response = "An error occurred while processing your message." });
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while sending message to the Python API.");
                return Json(new ChatResponse { Response = "An error occurred while sending message." });
            }
        }
        #endregion



        #region Chat Bot History
        public async Task<IActionResult> ChatBotHistoryUser(int page = 1, int pageSize = 10) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Fetching chat history for user ID: {userId}", userId);

            var chatHistory = await _chatBotHistoryService.GetUserChatHistoryAsync(userId, page, pageSize);

            return Json(chatHistory);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChatHistoryUser(int id) {
            _logger.LogInformation("Deleting chat history item with ID: {Id}", id);
            try {
                await _chatBotHistoryService.DeleteUserChatHistoryAsync(id);
                return Json(new { success = true });
            } catch (Exception ex) {
                _logger.LogError(ex, "Error deleting chat history item.");
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }






    public class ChatMessage {
        public string Message { get; set; }
    }

    public class ChatResponse {
        public string Response { get; set; }
    }

}
