using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class ManufacturerService {
        private readonly IDataAccess _dataAccess;

        public ManufacturerService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<Manufacturer>> GetAllManufacturersAsync() {
            string sql = $"SELECT * FROM Manufacturer";
            return await _dataAccess.GetAll<Manufacturer>(sql);
        }

        public async Task<Manufacturer?> GetManuFacturerById(int manufacturerID) {
            string sql = $"SELECT * FROM Manufacturer WHERE ManufacturerID = @ManufacturerID";
            return await _dataAccess.GetById<Manufacturer>(sql, new { ManufacturerID = manufacturerID });
        }

        public async Task<Manufacturer?> GetManufacturerByProductIdAsync(int productId)
        {
            string sql = @"
                SELECT m.* 
                FROM Manufacturer m
                INNER JOIN Product p ON m.ManufacturerID = p.ManufacturerID
                WHERE p.ProductID = @ProductID";
            return await _dataAccess.GetById<Manufacturer>(sql, new { ProductID = productId });
        }
    }
}
