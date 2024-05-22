using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class OrderItemService {
        private readonly IDataAccess _dataAccess;

        public OrderItemService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<OrderItem>> GetAllOrderItemAsync() {
            string sql = $"SELECT * FROM OrderItem";
            return await _dataAccess.GetAll<OrderItem>(sql);
        }

        public async Task<OrderItem?> GetOrderItemById(int orderItemID) {
            string sql = $"SELECT * FROM OrderItem WHERE OrderItemID = @OrderItemID";
            return await _dataAccess.GetById<OrderItem>(sql, new { OrderItemID = orderItemID });
        }
        public async Task<List<OrderItem>> GetOrderItemsByOrderId(int orderID) {
            string sql = $"SELECT * FROM OrderItem WHERE OrderID = @OrderID";
            return await _dataAccess.GetAll<OrderItem>(sql, new { OrderID = orderID });
        }


        public async Task InsertOrderItem(OrderItem orderItem) {
            string sql = $"INSERT INTO OrderItem (OrderID, ProductID, Quantity, Price) VALUES (@OrderID, @ProductID, @Quantity, @Price)";
            await _dataAccess.Insert(sql, orderItem);
        }

        public async Task UpdateOrderItem(OrderItem orderItem) {
            string sql = $"UPDATE OrderItem SET OrderID = @OrderID, ProductID = @ProductID, Quantity = @Quantity, Price = @Price WHERE OrderItemID = @OrderItemID";
            await _dataAccess.Update(sql, orderItem);
        }

        public async Task DeleteOrderItem(int orderItemID) {
            string sql = $"DELETE FROM OrderItem WHERE OrderItemID = @OrderItemID";
            await _dataAccess.Delete(sql, new { OrderItemID = orderItemID });
        }

    }
}
