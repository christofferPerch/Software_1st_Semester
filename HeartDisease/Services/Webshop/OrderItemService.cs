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
            string sql = $"INSERT INTO OrderItem (OrderID, ProductID, Quantity, PriceAtTimeOfOrder) VALUES (@OrderID, @ProductID, @Quantity, @PriceAtTimeOfOrder)";
            await _dataAccess.Insert(sql, orderItem);
        }

        public async Task UpdateOrderItem(OrderItem orderItem) {
            string sql = $"UPDATE OrderItem SET OrderID = @OrderID, ProductID = @ProductID, Quantity = @Quantity, Price = @Price WHERE OrderItemID = @OrderItemID";
            await _dataAccess.Update(sql, orderItem);
        }


        //public async Task DeleteOrderItemAsync(int orderItemID) {
        //    string sql = $"DELETE FROM OrderItem WHERE OrderItemID = @OrderItemID";
        //    await _dataAccess.Delete(sql, new { OrderItemID = orderItemID });
        //}

        public async Task DeleteOrderItemAsync(int orderItemID) {
            var orderItem = await _dataAccess.GetById<OrderItem>("SELECT * FROM OrderItem WHERE OrderItemID = @OrderItemID", new { OrderItemID = orderItemID });
            if (orderItem != null) {
                string sqlDelete = $"DELETE FROM OrderItem WHERE OrderItemID = @OrderItemID";
                await _dataAccess.Delete(sqlDelete, new { OrderItemID = orderItemID });

                // Update the total amount in the order table
                var order = await _dataAccess.GetById<Order>("SELECT * FROM [Order] WHERE OrderID = @OrderID", new { OrderID = orderItem.OrderID });
                if (order != null) {
                    order.TotalAmount -= orderItem.PriceAtTimeOfOrder * orderItem.Quantity;
                    string sqlUpdate = @"UPDATE [Order] SET TotalAmount = @TotalAmount WHERE OrderID = @OrderID";
                    await _dataAccess.Update(sqlUpdate, new { TotalAmount = order.TotalAmount, OrderID = order.OrderID });
                }
            }
        }


        public async Task AddProductToOrder(int orderId, int productId, int quantity, decimal price) {
            var orderItem = new OrderItem {
                OrderID = orderId,
                ProductID = productId,
                Quantity = quantity,
                PriceAtTimeOfOrder = price
            };
            await InsertOrderItem(orderItem);
        }

    }
}
