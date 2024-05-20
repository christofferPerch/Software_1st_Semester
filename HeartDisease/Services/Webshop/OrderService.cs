using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class OrderService {
        private readonly IDataAccess _dataAccess;

        public OrderService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<Order>> GetAllOrdersAsync() {
            string sql = $"SELECT * FROM Order";
            return await _dataAccess.GetAll<Order>(sql);
        }

        public async Task<Order?> GetOrderById(int orderId) {
            string sql = $"SELECT * FROM Order WHERE OrderID = @OrderID";
            return await _dataAccess.GetById<Order>(sql, new { OrderID = orderId });
        }

        public async Task InsertOrder(Order order) {
            var sql = @"INSERT INTO Order (UserId, OrderDate, TotalAmount) 
                        VALUES (@ChatBotId, @UserId, @OrderDate, @TotalAmount)";
            await _dataAccess.Insert(sql, order);
        }

        public async Task UpdateOrder(Order order) {
            var sql = @"UPDATE Order 
                        SET OrderDate = @OrderDate, TotalAmount = @TotalAmount, 
                            InitialMessage = @InitialMessage, GPTModelId = @GPTModelId 
                        WHERE OrderID = @OrderID";
            await _dataAccess.Update(sql, order);
        }

        public async Task DeleteOrder(int orderID) {
            var sql = "DELETE FROM Order WHERE OrderID = @OrderID";
            await _dataAccess.Delete(sql, new { OrderID = orderID });
        }

    }
}
