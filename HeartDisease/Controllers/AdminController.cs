using HeartDisease.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using HeartDisease.Utils;

namespace HeartDisease.Controllers {
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller {

        private readonly ILogger<AdminController> _logger;
        private readonly KnowledgeBaseService _knowledgeBaseService;

        public AdminController(ILogger<AdminController> logger, KnowledgeBaseService knowledgeBaseService)
        {
            _logger = logger;
            _knowledgeBaseService = knowledgeBaseService;
        }

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> KnowledgeBase()
        {
            var files = await _knowledgeBaseService.GetAllFilesAsync();
            return View(files);
        }
        public IActionResult ChatBotSettings()
        {
            return View();
        }
        public IActionResult ChatBotHistory()
        {
            return View();
        }
        public IActionResult MachineLearningModels()
        {
            return View();
        }
        public IActionResult DatabaseManagement()
        {
            return View();
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

    }
}
