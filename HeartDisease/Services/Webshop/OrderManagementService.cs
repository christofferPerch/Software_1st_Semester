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

    }
}
