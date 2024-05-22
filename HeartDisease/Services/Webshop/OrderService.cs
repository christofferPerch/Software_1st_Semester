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
                        VALUES (@UserId, @OrderDate, @TotalAmount)";
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

        //public async Task<Order> GetOrCreateOrderForUser(string userId) {
        //    // Attempt to find the most recent order for the user that does not have a matching entry in OrderHistory:
        //    string existingOrderSql = @"
        //        SELECT TOP 1 o.* FROM Order o
        //        LEFT JOIN OrderHistory oh ON o.OrderID = oh.OrderID
        //        WHERE o.UserId = @UserId AND oh.OrderID IS NULL
        //        ORDER BY o.OrderDate DESC";

        //    var order = await _dataAccess.GetById<Order>(existingOrderSql, new { UserId = userId });
        //    if (order != null) {
        //        return order;
        //    }

        //    // No open order found, create a new one:
        //    Order newOrder = new Order {
        //        UserId = userId,
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = 0.0m // Initially zero, update as items are added
        //    };

        //    string insertSql = @"
        //                        INSERT INTO Order (UserId, OrderDate, TotalAmount) 
        //                        OUTPUT INSERTED.OrderID
        //                        VALUES (@UserId, @OrderDate, @TotalAmount)";
        //    var orderId = await _dataAccess.InsertAndReturnId(insertSql, newOrder);
        //    return await GetOrderById(orderId);

        //}

    }
}
