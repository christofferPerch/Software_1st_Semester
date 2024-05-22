using HeartDisease.Models;
using HeartDisease.Models.Webshop;
using HeartDisease.Services;
using HeartDisease.Services.Webshop;
using HeartDisease.Utils;
using HeartDisease.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

namespace HeartDisease.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly ILogger<AdminController> _logger;
        private readonly KnowledgeBaseService _knowledgeBaseService;
        private readonly ChatBotService _chatBotService;
        private readonly ChatBotHistoryService _chatBotHistoryService;
        private readonly DatabaseManagementService _databaseManagementService;
        private readonly OrderManagementService _orderManagementService;
        private readonly HttpClient _httpClient;
        public AdminController(ILogger<AdminController> logger, KnowledgeBaseService knowledgeBaseService,
            ChatBotService chatBotService, ChatBotHistoryService chatBotHistoryService, DatabaseManagementService databaseManagementService, OrderManagementService orderManagementService)
        {
            _logger = logger;
            _knowledgeBaseService = knowledgeBaseService;
            _chatBotService = chatBotService;
            _httpClient = new HttpClient();
            _chatBotHistoryService = chatBotHistoryService;
            _databaseManagementService = databaseManagementService;
            _orderManagementService = orderManagementService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Knowledge Base

        public async Task<IActionResult> KnowledgeBase()
        {
            var files = await _knowledgeBaseService.GetAllFilesAsync();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> TrainChatbot()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync("http://127.0.0.1:5000/genai_embed", null);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Chatbot training started successfully.";
                        var lastTrained = DateTime.Now;
                        await _chatBotService.UpdateChatBotLastTrained(1, lastTrained);
                    } else
                    {
                        TempData["ErrorMessage"] = "Failed to start chatbot training.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
            return RedirectToAction("KnowledgeBase");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "No file selected.";
                return RedirectToAction("KnowledgeBase");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var fileId = await _knowledgeBaseService.UploadFileAsync(file.FileName, stream);
                    TempData["SuccessMessage"] = $"File uploaded successfully! File ID: {fileId}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                TempData["ErrorMessage"] = $"Error uploading file: {ex.Message}";
            }
            return RedirectToAction("KnowledgeBase");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFileById(string fileId)
        {
            try
            {
                await _knowledgeBaseService.DeleteFileByIdAsync(fileId);
                TempData["SuccessMessage"] = "File deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting file: {ex.Message}";
            }
            return RedirectToAction("KnowledgeBase");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFileByName(string filename)
        {
            try
            {
                await _knowledgeBaseService.DeleteFileByNameAsync(filename);
                TempData["SuccessMessage"] = "File deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting file: {ex.Message}";
            }
            return RedirectToAction("KnowledgeBase");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            try
            {
                var objectId = new ObjectId(fileId);
                var (fileStream, fileName) = await _knowledgeBaseService.GetFileAsync(objectId);
                if (fileStream == null)
                {
                    TempData["ErrorMessage"] = "File not found.";
                    return RedirectToAction("KnowledgeBase");
                }

                var contentType = FileHelper.GetMimeType(fileName);
                return File(fileStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error downloading file: {ex.Message}";
                return RedirectToAction("KnowledgeBase");
            }
        }

        #endregion

        #region Chat Bot Settings

        public async Task<IActionResult> ChatBotSettings()
        {
            var (settings, lastTrained) = await _chatBotService.GetChatBotSettingsWithLastTrained(1);
            var models = await _chatBotService.GetAllGPTModels();
            ViewBag.Models = new SelectList(models, "Id", "ModelName");

            var viewModel = new ChatBotSettingsViewModel
            {
                Settings = settings,
                LastTrained = lastTrained
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChatBotSettings(ChatBotSettings settings)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _chatBotService.UpdateChatBotSettings(settings);
                    TempData["SuccessMessage"] = "ChatBot settings updated successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating ChatBot settings: {ex.Message}";
                }

                return RedirectToAction("ChatBotSettings");
            }

            var models = await _chatBotService.GetAllGPTModels();
            ViewBag.Models = new SelectList(models, "Id", "ModelName");
            return View("ChatBotSettings", settings);
        }


        #endregion

        #region Chat Bot History
        public async Task<IActionResult> ChatBotHistory(int page = 1, int pageSize = 10, string searchText = "")
        {
            ViewBag.SearchText = searchText;

            var histories = await _chatBotHistoryService.GetRelevantHistory(searchText);

            var totalItems = histories.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedHistories = histories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new ChatBotHistoryViewModel
            {
                ChatHistories = paginatedHistories,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchChatBotHistory(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                TempData["ErrorMessage"] = "Search text cannot be empty.";
                return RedirectToAction("ChatBotHistory");
            }

            try
            {
                var results = await _chatBotHistoryService.GetRelevantHistory(searchText);
                return View("ChatBotHistory", results);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error searching chat bot history: {ex.Message}";
                return RedirectToAction("ChatBotHistory");
            }
        }


        #endregion

        #region Machine Learning Models
        public IActionResult MachineLearningModels()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RetrainModelLR()
        {
            return await RetrainModel("http://127.0.0.1:5000/retrain_lr", "Logistic Regression");
        }

        [HttpPost]
        public async Task<IActionResult> RetrainModelRF()
        {
            return await RetrainModel("http://127.0.0.1:5000/retrain_rf", "Random Forest");
        }

        [HttpPost]
        public async Task<IActionResult> RetrainModelTF()
        {
            return await RetrainModel("http://127.0.0.1:5000/retrain_tf", "TensorFlow");
        }

        private async Task<IActionResult> RetrainModel(string url, string modelName)
        {
            _logger.LogInformation($"Initiating {modelName} model retraining");

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{modelName} model retrained successfully");
                    return Json(new { message = $"{modelName} model retrained and saved successfully." });
                } else
                {
                    _logger.LogError("Error response from Python API: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return Json(new { message = $"Failed to retrain {modelName} model." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retraining the {modelName} model.");
                return Json(new { message = $"An error occurred while retraining the {modelName} model." });
            }
        }

        #endregion

        #region Database Mangement 
        public async Task<IActionResult> DatabaseManagement()
        {
            var latestFragmentation = await _databaseManagementService.GetLatestIndexFragmentation();
            return View(latestFragmentation);
        }

        [HttpPost]
        public async Task<IActionResult> CheckIndexFragmentation()
        {
            try
            {
                await _databaseManagementService.CheckIndexFragmentation(DateTime.Now);
                TempData["SuccessMessage"] = "Index fragmentation check completed successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
            return RedirectToAction("DatabaseManagement");
        }

        [HttpPost]
        public async Task<IActionResult> RebuildIndex(string indexName)
        {
            try
            {
                await _databaseManagementService.RebuildIndex(indexName);
                TempData["SuccessMessage"] = $"Index '{indexName}' rebuilt successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
            return RedirectToAction("DatabaseManagement");
        }

        [HttpPost]
        public async Task<IActionResult> ReorganizeIndex(string indexName)
        {
            try
            {
                await _databaseManagementService.ReorganizeIndex(indexName);
                TempData["SuccessMessage"] = $"Index '{indexName}' reorganized successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }
            return RedirectToAction("DatabaseManagement");
        }
        #endregion

        #region Order Management
        [HttpGet]
        public async Task<IActionResult> OrderManagement()
        {
            var orders = await _orderManagementService.GetAllOrdersAsync();
            var orderHistories = new Dictionary<int, List<OrderHistory>>();

            foreach (var order in orders)
            {
                var histories = await _orderManagementService.GetOrderHistoryByOrderIdAsync(order.OrderID);
                orderHistories.Add(order.OrderID, histories);
            }

            var viewModel = new OrderManagementViewModel
            {
                Orders = orders,
                OrderHistories = orderHistories
            };

            return View(viewModel);
        }
    }

    #endregion
}
