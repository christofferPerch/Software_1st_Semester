using HeartDisease.Models;

namespace HeartDisease.ViewModels
{
    public class ChatBotSettingsViewModel
    {
        public ChatBot ChatBot { get; set; }
        public ChatBotSettings ChatBotSettings { get; set; }
        public List<GPTModel> GPTModels { get; set; }
    }
}
