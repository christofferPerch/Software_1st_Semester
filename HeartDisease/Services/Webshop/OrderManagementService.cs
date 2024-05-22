using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class OrderManagementService {
        private readonly IDataAccess _dataAccess;
        public OrderManagementService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            string sql = $"SELECT * FROM [Order]";
            return await _dataAccess.GetAll<Order>(sql);
        }
        public async Task<List<OrderHistory>> GetOrderHistoryByOrderIdAsync(int orderId)
        {
            string sql = $"SELECT * FROM OrderHistory WHERE OrderID = @OrderId";
            var parameters = new { OrderId = orderId };
            return await _dataAccess.GetAll<OrderHistory>(sql, parameters);
        }
        public async Task<int> UpdateOrderHistoryStatus(int orderId)
        {
            var parameters = new { OrderId = orderId };
            return await _dataAccess.Update("UPDATE OrderHistory SET Status = 'completed' WHERE OrderID = @OrderId", parameters);

        }

    }
}
