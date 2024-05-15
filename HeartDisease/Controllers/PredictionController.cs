using HeartDisease.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HeartDisease.Controllers
{
    [Authorize]
    public class PredictionController : Controller
    {
        private readonly ILogger<PredictionController> _logger;
        private readonly HttpClient _httpClient;

        public PredictionController(ILogger<PredictionController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public IActionResult Index(string predictionResult = null)
        {
            ViewBag.PredictionResult = predictionResult;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Predict(PredictionModel model)
        {
            _logger.LogInformation("Received form submission with model: {@Model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid: {@ModelState}", ModelState);
                return Json(new { error = "Model state is invalid." });
            }

            string apiEndpoint = model.SelectedModel.SelectedModel switch
            {
                "predict_lr" => "http://127.0.0.1:5000/predict_lr",
                "predict_rf" => "http://127.0.0.1:5000/predict_rf",
                "predict_tf" => "http://127.0.0.1:5000/predict_tf",
                _ => throw new ArgumentException("Invalid model selection")
            };

            var jsonContent = JsonConvert.SerializeObject(new
            {
                model.Sex,
                model.GeneralHealth,
                model.PhysicalHealthDays,
                model.MentalHealthDays,
                model.PhysicalActivities,
                model.SleepHours,
                model.HadAsthma,
                model.HadDepressiveDisorder,
                model.HadKidneyDisease,
                model.HadDiabetes,
                model.DifficultyWalking,
                model.SmokerStatus,
                model.ECigaretteUsage,
                model.RaceEthnicityCategory,
                model.AgeCategory,
                model.BMI,
                model.AlcoholDrinkers,
                model.HIVTesting
            });
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to Python API: {JsonContent}", jsonContent);

            try
            {
                var response = await _httpClient.PostAsync(apiEndpoint, contentString);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received response from Python API: {ResponseString}", responseString);
                    var predictionResult = JsonConvert.DeserializeObject<PredictionResultModel>(responseString);
                    return Json(new { prediction = predictionResult.Prediction });
                }
                else
                {
                    _logger.LogError("Error response from Python API: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return Json(new { error = "An error occurred while predicting." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending data to the Python API.");
                return Json(new { error = "An error occurred while predicting." });
            }
        }
    }
}
