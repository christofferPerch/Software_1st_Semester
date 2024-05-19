using HeartDisease.DataAccess;
using HeartDisease.Models;

namespace HeartDisease.Services {
    public class ChatBotHistoryService {
        private readonly IDataAccess _dataAccess;

        public ChatBotHistoryService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<ChatBotHistory>> GetPaginatedChatHistoryAsync(int page, int pageSize) {
            var sql = @"SELECT * FROM ChatBotHistory
                        ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var parameters = new { Offset = (page - 1) * pageSize, PageSize = pageSize };
            return await _dataAccess.GetAll<ChatBotHistory>(sql, parameters);
        }

        public async Task<int> GetChatHistoryCountAsync() {
            var sql = "SELECT COUNT(*) FROM ChatBotHistory";
            return await _dataAccess.ExecuteScalar<int>(sql);
        }

        public async Task InsertChatHistoryAsync(ChatBotHistory chatBotHistory) {
            var sql = @"INSERT INTO ChatBotHistory (ChatBotId, Message, Response, UserId) 
                        VALUES (@ChatBotId, @Message, @Response, @UserId)";
            await _dataAccess.Insert(sql, chatBotHistory);
        }

        //public async Task InsertChatHistoryAsync(ChatBotHistory chatBotHistory) {
        //    var sql = @"INSERT INTO ChatBotHistory (ChatBotId, Message, Response) 
        //                VALUES (@ChatBotId, @Message, @Response)";
        //    await _dataAccess.Insert(sql, chatBotHistory);
        //}



        #region Chat Bot History for User
        public async Task<List<ChatBotHistory>> GetUserChatHistoryAsync(string userId, int page, int pageSize) {
            var sql = @"SELECT * FROM ChatBotHistory WHERE UserId = @UserId
                ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            var parameters = new { UserId = userId, Offset = (page - 1) * pageSize, PageSize = pageSize };
            return await _dataAccess.GetAll<ChatBotHistory>(sql, parameters);
        }

        public async Task<int> GetUserChatHistoryCountAsync(string userId) {
            var sql = "SELECT COUNT(*) FROM ChatBotHistory WHERE UserId = @UserId";
            return await _dataAccess.ExecuteScalar<int>(sql, new { UserId = userId });
        }

        public async Task DeleteUserChatHistoryAsync(int id) {
            var sql = "DELETE FROM ChatBotHistory WHERE Id = @Id";
            await _dataAccess.Delete(sql, new { Id = id });
        }


        #endregion
    }
}
