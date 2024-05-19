namespace HeartDisease.Models {
    public class PredictionModel {
        public List<string> Sex { get; set; }
        public List<string> GeneralHealth { get; set; }
        public List<int> PhysicalHealthDays { get; set; }
        public List<int> MentalHealthDays { get; set; }
        public List<string> PhysicalActivities { get; set; }
        public List<int> SleepHours { get; set; }
        public List<string> HadAsthma { get; set; }
        public List<string> HadDepressiveDisorder { get; set; }
        public List<string> HadKidneyDisease { get; set; }
        public List<string> HadDiabetes { get; set; }
        public List<string> DifficultyWalking { get; set; }
        public List<string> SmokerStatus { get; set; }
        public List<string> ECigaretteUsage { get; set; }
        public List<string> RaceEthnicityCategory { get; set; }
        public List<string> AgeCategory { get; set; }
        public List<double> BMI { get; set; }
        public List<string> AlcoholDrinkers { get; set; }
        public List<string> HIVTesting { get; set; }

        public SelectedModelModel SelectedModel { get; set; }

        public class SelectedModelModel {
            public string SelectedModel { get; set; }
        }
    }
}
