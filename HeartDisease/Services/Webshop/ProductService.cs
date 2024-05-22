using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class ProductService {
        private readonly IDataAccess _dataAccess;

        public ProductService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<Product>> GetAllProductsAsync() {
            string sql = $"SELECT * FROM Product";
            return await _dataAccess.GetAll<Product>(sql);
        }

        public async Task<Product?> GetProductByIdAsync(int productId) {
            string sql = $"SELECT * FROM Product WHERE ProductID = @ProductID";
            return await _dataAccess.GetById<Product>(sql, new { ProductID = productId });
        }

        // Using this
        public async Task<List<Product>> GetProductsPagesAsync(int pageNumber, int pageSize) {
            string sql = $@"SELECT * FROM Product 
                            ORDER BY ProductID 
                            OFFSET {pageSize * (pageNumber - 1)} ROWS 
                            FETCH NEXT {pageSize} ROWS ONLY;";
            return await _dataAccess.GetAll<Product>(sql);
        }

        public async Task<int> CountProductsAsync() {
            string sql = "SELECT COUNT(*) FROM Product;";
            return await _dataAccess.ExecuteScalar<int>(sql);
        }
    }

}

