using HeartDisease.DataAccess;
using HeartDisease.Models;

namespace HeartDisease.Services {
    public class ChatBotService {
        private readonly IDataAccess _dataAccess;

        public ChatBotService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<(ChatBotSettings?, DateTime?)> GetChatBotSettingsWithLastTrained(int id) {
            var sqlSettings = @"
                SELECT 
                    s.*, 
                    m.Id as ModelId, m.ModelName
                FROM ChatBotSettings s
                LEFT JOIN GPTModel m ON s.GPTModelId = m.Id
                WHERE s.Id = @Id";
            var parameters = new { Id = id };

            var settings = await _dataAccess.GetById<ChatBotSettings>(sqlSettings, parameters);

            var sqlLastTrained = @"
                SELECT LastTrained
                FROM ChatBot
                WHERE ChatBotSettingsId = @Id";
            var lastTrained = await _dataAccess.GetById<DateTime?>(sqlLastTrained, parameters);

            return (settings, lastTrained);
        }

        public async Task UpdateChatBotSettings(ChatBotSettings settings) {
            var sql = @"UPDATE ChatBotSettings 
                        SET BasePrompt = @BasePrompt, Temperature = @Temperature, 
                            InitialMessage = @InitialMessage, GPTModelId = @GPTModelId 
                        WHERE Id = @Id";
            await _dataAccess.Update(sql, settings);
        }

        public async Task UpdateChatBot(ChatBot chatBot) {
            var sql = @"UPDATE ChatBot 
                        SET ChatBotSettingsId = @ChatBotSettingsId, LastTrained = @LastTrained 
                        WHERE Id = @Id";
            await _dataAccess.Update(sql, chatBot);
        }

        public async Task<List<GPTModel>> GetAllGPTModels() {
            var sql = "SELECT * FROM GPTModel";
            return await _dataAccess.GetAll<GPTModel>(sql);
        }

        public async Task UpdateChatBotLastTrained(int chatBotSettingsId, DateTime lastTrained) {
            var sql = @"UPDATE ChatBot 
                SET LastTrained = @LastTrained 
                WHERE ChatBotSettingsId = @ChatBotSettingsId";
            var parameters = new { ChatBotSettingsId = chatBotSettingsId, LastTrained = lastTrained };
            await _dataAccess.Update(sql, parameters);
        }

    }
}
