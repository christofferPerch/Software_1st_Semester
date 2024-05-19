namespace HeartDisease.Models
{
    public class ChatBotHistory
    {
        public int Id { get; set; }
        public int ChatBotId { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }
    }
}
