namespace HeartDisease.Models
{
    public class ChatBotSettings
    {
        public int Id { get; set; }
        public string BasePrompt { get; set; }
        public double Temperature { get; set; }  
        public string InitialMessage { get; set; }
        public int GPTModelId { get; set; }
    }
}
