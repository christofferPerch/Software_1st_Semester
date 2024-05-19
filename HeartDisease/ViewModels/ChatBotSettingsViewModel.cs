using HeartDisease.Models;

namespace HeartDisease.ViewModels {
    public class ChatBotSettingsViewModel {
        public ChatBotSettings Settings { get; set; }
        public DateTime? LastTrained { get; set; }
    }
}