namespace HeartDisease.Models.Webshop {
    public class Review {

        public int ReviewID { get; set; }
        public int ProductID { get; set; }
        public int ExcellentReviewPercent { get; set; }
        public int AverageReviewPercent { get; set; }
        public int PoorReviewPercent { get; set; }

    }
}
