namespace HeartDisease.Models {
    public class FragmentationHistory {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public float FragmentationPercent { get; set; }
        public DateTime CheckDate { get; set; }

    }
}
