using HeartDisease.DataAccess;
using HeartDisease.Models;

namespace HeartDisease.Services {
    public class PredictionService {

        private readonly IDataAccess _dataAccess;
        private readonly ILogger<PredictionService> _logger;

        public PredictionService(ILogger<PredictionService> logger, IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _logger = logger;
        }

        public async Task<List<PredictionModel>> GetAllPredictionParameters() {
            string SqlStr = @"SELECT TOP (10) *
                             FROM HearthSurvey";

            return await _dataAccess.GetAll<PredictionModel>(SqlStr);
        }


        public async Task<int> InsertPredictionForUser(PredictionModel model, string userId) {
            const string sql = @"
            INSERT INTO HealthSurvey 
            (Sex, GeneralHealth, PhysicalHealthDays, MentalHealthDays, PhysicalActivities, SleepHours, HadAsthma, 
            HadDepressiveDisorder, HadKidneyDisease, HadDiabetes, DifficultyWalking, SmokerStatus, ECigaretteUsage, RaceEthnicityCategory, AgeCategory, BMI, AlcoholDrinkers, HIVTesting, UserId)
            VALUES 
            (@Sex, @GeneralHealth, @PhysicalHealthDays, @MentalHealthDays, @PhysicalActivities, @SleepHours, @HadAsthma, 
                    @HadDepressiveDisorder, @HadKidneyDisease, @HadDiabetes, @DifficultyWalking, @SmokerStatus, @ECigaretteUsage, @RaceEthnicityCategory, @AgeCategory, @BMI, @AlcoholDrinkers, @HIVTesting, @UserId);
                ";
            try {
                var result = await _dataAccess.Insert(sql, new {
                    Sex = model.Sex?.FirstOrDefault(),
                    GeneralHealth = model.GeneralHealth?.FirstOrDefault(),
                    PhysicalHealthDays = model.PhysicalHealthDays?.FirstOrDefault(),
                    MentalHealthDays = model.MentalHealthDays?.FirstOrDefault(),
                    PhysicalActivities = model.PhysicalActivities?.FirstOrDefault(),
                    SleepHours = model.SleepHours?.FirstOrDefault(),
                    HadAsthma = model.HadAsthma?.FirstOrDefault(),
                    HadDepressiveDisorder = model.HadDepressiveDisorder?.FirstOrDefault(),
                    HadKidneyDisease = model.HadKidneyDisease?.FirstOrDefault(),
                    HadDiabetes = model.HadDiabetes?.FirstOrDefault(),
                    DifficultyWalking = model.DifficultyWalking?.FirstOrDefault(),
                    SmokerStatus = model.SmokerStatus?.FirstOrDefault(),
                    ECigaretteUsage = model.ECigaretteUsage?.FirstOrDefault(),
                    RaceEthnicityCategory = model.RaceEthnicityCategory?.FirstOrDefault(),
                    AgeCategory = model.AgeCategory?.FirstOrDefault(),
                    BMI = model.BMI?.FirstOrDefault(),
                    AlcoholDrinkers = model.AlcoholDrinkers?.FirstOrDefault(),
                    HIVTesting = model.HIVTesting?.FirstOrDefault(),
                    UserId = userId,
                });
                _logger.LogInformation("Inserted {Count} records into the database successfully.", result);
                return result;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error occurred while inserting data into the database.");
                throw;
            }
        }
    }

}

