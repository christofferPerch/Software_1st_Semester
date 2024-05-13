namespace HeartDisease.Models
{
    public class PredictionModel
    {
        public string ModelType { get; set; }
        public string Sex { get; set; }
        public string GeneralHealth { get; set; }
        public int PhysicalHealthDays { get; set; }
        public int MentalHealthDays { get; set; }
        public string PhysicalActivities { get; set; }
        public int SleepHours { get; set; }
        public string HadAsthma { get; set; }
        public string HadDepressiveDisorder { get; set; }
        public string HadKidneyDisease { get; set; }
        public string HadDiabetes { get; set; }
        public string DifficultyWalking { get; set; }
        public string SmokerStatus { get; set; }
        public string ECigaretteUsage { get; set; }
        public string RaceEthnicityCategory { get; set; }
        public string AgeCategory { get; set; }
        public double BMI { get; set; }
        public string AlcoholDrinkers { get; set; }
        public string HIVTesting { get; set; }

        public List<string> ModelTypes => new List<string> { "Logistic Regression", "Random Forest", "TensorFlow" };
        public List<string> SexOptions => new List<string> { "Male", "Female" };
        public List<string> GeneralHealthOptions => new List<string> { "Poor", "Fair", "Good", "Very good", "Excellent" };
        public List<string> AgeCategories => new List<string> {
        "Age 18 to 24", "Age 25 to 29", "Age 30 to 34", "Age 35 to 39",
        "Age 40 to 44", "Age 45 to 49", "Age 50 to 54", "Age 55 to 59",
        "Age 60 to 64", "Age 65 to 69", "Age 70 to 74", "Age 75 to 79",
        "Age 80 or older"
    };
        public List<string> RaceEthnicityCategories => new List<string> {
        "Black only, Non-Hispanic",
        "Hispanic",
        "Multiracial, Non-Hispanic",
        "Other race only, Non-Hispanic",
        "White only, Non-Hispanic",
    };
    }

}
