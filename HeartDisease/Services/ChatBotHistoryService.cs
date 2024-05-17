using HeartDisease.DataAccess;
using HeartDisease.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeartDisease.Services
{
    public class ChatBotHistoryService
    {
        private readonly IDataAccess _dataAccess;

        public ChatBotHistoryService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<ChatBotHistory>> GetPaginatedChatHistoryAsync(int page, int pageSize)
        {
            var sql = @"SELECT * FROM ChatBotHistory
                        ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var parameters = new { Offset = (page - 1) * pageSize, PageSize = pageSize };
            return await _dataAccess.GetAll<ChatBotHistory>(sql, parameters);
        }

        public async Task<int> GetChatHistoryCountAsync()
        {
            var sql = "SELECT COUNT(*) FROM ChatBotHistory";
            return await _dataAccess.ExecuteScalar<int>(sql);
        }

        public async Task InsertChatHistoryAsync(ChatBotHistory chatBotHistory)
        {
            var sql = @"INSERT INTO ChatBotHistory (ChatBotId, Message, Response) 
                        VALUES (@ChatBotId, @Message, @Response)";
            await _dataAccess.Insert(sql, chatBotHistory);
        }
    }
}
