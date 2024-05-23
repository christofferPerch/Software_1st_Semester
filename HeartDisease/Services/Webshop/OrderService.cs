using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class OrderService {
        private readonly IDataAccess _dataAccess;

        public OrderService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<Order>> GetAllOrdersAsync() {
            string sql = $"SELECT * [Order] Order";
            return await _dataAccess.GetAll<Order>(sql);
        }

        public async Task<Order?> GetOrderById(int orderId) {
            string sql = $"SELECT * FROM [Order] WHERE OrderID = @OrderID";
            return await _dataAccess.GetById<Order>(sql, new { OrderID = orderId });
        }

        public async Task InsertOrder(Order order) {
            var sql = @"INSERT INTO [Order] (UserId, OrderDate, TotalAmount) 
                        VALUES (@UserId, @OrderDate, @TotalAmount)";
            await _dataAccess.Insert(sql, order);
        }

        public async Task UpdateOrder(Order order) {
            var sql = @"UPDATE [Order] 
                        SET UserId = @UserId, OrderDate = @OrderDate, 
                            TotalAmount = @TotalAmount
                        WHERE OrderID = @OrderID";
            await _dataAccess.Update(sql, order);
        }

        public async Task DeleteOrder(int orderID) {
            var sql = "DELETE FROM [Order] WHERE OrderID = @OrderID";
            await _dataAccess.Delete(sql, new { OrderID = orderID });
        }

        public async Task<Order?> GetActiveOrderForUser(string userId) {
            string sql = @"
                SELECT TOP 1 * FROM [Order]
                LEFT JOIN OrderHistory ON [Order].OrderID = OrderHistory.OrderID
                WHERE UserId = @UserId AND OrderHistory.OrderID IS NULL
                ORDER BY OrderDate DESC";
            return await _dataAccess.GetById<Order>(sql, new { UserId = userId });
        }

        // Creates a new order if there isn't an active one
        public async Task<Order> EnsureActiveOrder(string userId) {
            var order = await GetActiveOrderForUser(userId);
            if (order != null) return order;

            order = new Order {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = 0m
            };
            await InsertOrder(order);
            return await GetActiveOrderForUser(userId); // Re-fetch to ensure we have the ID
        }



    }
}

