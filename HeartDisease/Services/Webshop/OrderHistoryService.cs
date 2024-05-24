using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class OrderHistoryService {
        private readonly IDataAccess _dataAccess;

        public OrderHistoryService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<OrderHistory>> GetAllOrderHistoriesAsync() {
            string sql = $"SELECT * FROM OrderHistory";
            return await _dataAccess.GetAll<OrderHistory>(sql);
        }

        public async Task<OrderHistory?> GetOrderHistoryById(int entryID) {
            string sql = $"SELECT * FROM OrderHistory WHERE EntryID = @EntryID";
            return await _dataAccess.GetById<OrderHistory>(sql, new { EntryID = entryID });
        }

        public async Task UpdateOrderHistoryStatus(int entryID, bool status) {
            var sql = @"UPDATE OrderHistory 
                        SET Status = @Status 
                        WHERE EntryID = @EntryID";
            await _dataAccess.Update(sql, new { EntryID = entryID, Status = status });
        }


    }
}
