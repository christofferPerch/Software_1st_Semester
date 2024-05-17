using HeartDisease.DataAccess;
using HeartDisease.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartDisease.Services
{
    public class ChatBotService
    {
        private readonly IDataAccess _dataAccess;

        public ChatBotService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<ChatBotSettings?> GetChatBotSettings(int id)
        {
            var sql = @"
                SELECT 
                    s.*, 
                    m.Id as ModelId, m.ModelName
                FROM ChatBotSettings s
                LEFT JOIN GPTModel m ON s.GPTModelId = m.Id
                WHERE s.Id = @Id";
            var parameters = new { Id = id };

            var settings = await _dataAccess.GetById<ChatBotSettings>(sql, parameters);
            return settings;
        }

        public async Task UpdateChatBotSettings(ChatBotSettings settings)
        {
            var sql = @"UPDATE ChatBotSettings 
                        SET BasePrompt = @BasePrompt, Temperature = @Temperature, 
                            InitialMessage = @InitialMessage, GPTModelId = @GPTModelId 
                        WHERE Id = @Id";
            await _dataAccess.Update(sql, settings);
        }

        public async Task UpdateChatBot(ChatBot chatBot)
        {
            var sql = @"UPDATE ChatBot 
                        SET ChatBotSettingsId = @ChatBotSettingsId, LastTrained = @LastTrained 
                        WHERE Id = @Id";
            await _dataAccess.Update(sql, chatBot);
        }

        public async Task<List<GPTModel>> GetAllGPTModels()
        {
            var sql = "SELECT * FROM GPTModel";
            return await _dataAccess.GetAll<GPTModel>(sql);
        }
    }
}
