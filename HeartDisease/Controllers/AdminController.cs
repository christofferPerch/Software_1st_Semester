using HeartDisease.Services;
using HeartDisease.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using HeartDisease.Utils;
using HeartDisease.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HeartDisease.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly ILogger<AdminController> _logger;
        private readonly KnowledgeBaseService _knowledgeBaseService;
        private readonly ChatBotService _chatBotService;
        private readonly ChatBotHistoryService _chatBotHistoryService;
        private readonly DatabaseManagementService _databaseManagementService;
        public AdminController(ILogger<AdminController> logger, KnowledgeBaseService knowledgeBaseService,
            ChatBotService chatBotService, ChatBotHistoryService chatBotHistoryService, DatabaseManagementService databaseManagementService )
        {
            _logger = logger;
            _knowledgeBaseService = knowledgeBaseService;
            _chatBotService = chatBotService;
            _chatBotHistoryService = chatBotHistoryService;
            _databaseManagementService = databaseManagementService;
        }

        public IActionResult Index() {
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
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://127.0.0.1:5000/genai_embed", null);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Chatbot training started successfully.";
                    var lastTrained = DateTime.Now;
                    await _chatBotService.UpdateChatBotLastTrained(1, lastTrained);
                }
                else
                {
                    TempData["Message"] = "Failed to start chatbot training.";
                }
            }
            return RedirectToAction("KnowledgeBase");
        }


        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return View("KnowledgeBase", model: "No file selected");
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var fileId = await _knowledgeBaseService.UploadFileAsync(file.FileName, stream);
                    return RedirectToAction("KnowledgeBase", new { message = $"File uploaded successfully! File ID: {fileId}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error uploading file: {ex.Message}");
                return View("KnowledgeBase", model: $"Error uploading file: {ex.Message}");
            }
        }

        [HttpPost("delete-file-by-id")]
        public async Task<IActionResult> DeleteFileById(string fileId)
        {
            await _knowledgeBaseService.DeleteFileByIdAsync(fileId);
            return RedirectToAction("KnowledgeBase");
        }

        [HttpPost("delete-file-by-name")]
        public async Task<IActionResult> DeleteFileByName(string filename)
        {
            await _knowledgeBaseService.DeleteFileByNameAsync(filename);
            return RedirectToAction("KnowledgeBase");
        }

        [HttpGet("download-file/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var objectId = new ObjectId(fileId);
            var (fileStream, fileName) = await _knowledgeBaseService.GetFileAsync(objectId);
            if (fileStream == null)
            {
                return NotFound("File not found.");
            }

            var contentType = FileHelper.GetMimeType(fileName);
            return File(fileStream, contentType, fileName);
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
                await _chatBotService.UpdateChatBotSettings(settings);
                return RedirectToAction("ChatBotSettings");
            }
            var models = await _chatBotService.GetAllGPTModels();
            ViewBag.Models = new SelectList(models, "Id", "ModelName");
            return View("ChatBotSettings", settings);
        }

        #endregion

        #region Chat Bot History
        public async Task<IActionResult> ChatBotHistory(int page = 1, int pageSize = 10)
        {
            var chatHistory = await _chatBotHistoryService.GetPaginatedChatHistoryAsync(page, pageSize);
            var totalItems = await _chatBotHistoryService.GetChatHistoryCountAsync();

            var viewModel = new ChatBotHistoryViewModel
            {
                ChatHistories = chatHistory,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            return View(viewModel);
        }


        #endregion

        public IActionResult MachineLearningModels()
        {
            return View();
        }

        #region Database Mangement 

        public IActionResult DatabaseManagement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckFragmentation()
        {
            var fragmentationData = await _databaseManagementService.CheckFragmentationAsync();
            _logger.LogInformation("Fragmentation data count: {Count}", fragmentationData.Count); // Logging for debug
            return PartialView("_FragmentationResults", fragmentationData);
        }



        [HttpPost]
        public async Task<IActionResult> RebuildIndexes(string tableName)
        {
            await _databaseManagementService.RebuildIndexesAsync(tableName);
            TempData["Message"] = "Indexes rebuilt successfully.";
            return RedirectToAction("DatabaseManagement");
        }

        [HttpPost]
        public async Task<IActionResult> ReorganizeIndexes(string tableName)
        {
            await _databaseManagementService.ReorganizeIndexesAsync(tableName);
            TempData["Message"] = "Indexes reorganized successfully.";
            return RedirectToAction("DatabaseManagement");
        }

        #endregion
    }
}
