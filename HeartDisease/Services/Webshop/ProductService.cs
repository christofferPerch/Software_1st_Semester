using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;



namespace HeartDisease.Services.Webshop
{
    public class ProductService
    {
        private readonly IDataAccess _dataAccess;

        public ProductService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            string sql = $"SELECT * FROM Product";
            return await _dataAccess.GetAll<Product>(sql);
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            string sql = $"SELECT * FROM Product WHERE ProductID = @ProductID";
            return await _dataAccess.GetById<Product>(sql, new { ProductID = productId });
        }

        public async Task<List<Manufacturer>> GetManufacturersByName(string name)
        {
            var query = @"
                SELECT TOP 5 * 
                FROM Manufacturer 
                WHERE Name LIKE @Name + '%' 
                ORDER BY Name";
            var parameters = new { Name = name };
            return await _dataAccess.GetAll<Manufacturer>(query, parameters);
        }

        public async Task<List<Product>> GetProductsPagesAsync(int pageNumber, int pageSize, string searchTerm, int[] manufacturerIds, int minPrice, int maxPrice)
        {
            var sql = new StringBuilder(@"SELECT * FROM Product WHERE (@SearchTerm IS NULL OR ProductName LIKE '%' + @SearchTerm + '%')");

            if (manufacturerIds != null && manufacturerIds.Length > 0)
            {
                sql.Append(" AND ManufacturerID IN @ManufacturerIds");
            }

            sql.Append(" AND Price BETWEEN @MinPrice AND @MaxPrice");

            sql.Append(" ORDER BY ProductID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;");

            var parameters = new
            {
                SearchTerm = searchTerm,
                ManufacturerIds = manufacturerIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Offset = pageSize * (pageNumber - 1),
                PageSize = pageSize
            };

            return await _dataAccess.GetAll<Product>(sql.ToString(), parameters);
        }

        public async Task<int> CountProductsAsync(string searchTerm, int[] manufacturerIds, int minPrice, int maxPrice)
        {
            var sql = new StringBuilder(@"SELECT COUNT(*) FROM Product WHERE (@SearchTerm IS NULL OR ProductName LIKE '%' + @SearchTerm + '%')");

            if (manufacturerIds != null && manufacturerIds.Length > 0)
            {
                sql.Append(" AND ManufacturerID IN @ManufacturerIds");
            }

            sql.Append(" AND Price BETWEEN @MinPrice AND @MaxPrice");

            var parameters = new
            {
                SearchTerm = searchTerm,
                ManufacturerIds = manufacturerIds,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return await _dataAccess.ExecuteScalar<int>(sql.ToString(), parameters);
        }


        public async Task<Manufacturer> GetManufacturerByIdAsync(int manufacturerId)
        {
            string sql = "SELECT * FROM Manufacturer WHERE ManufacturerID = @ManufacturerID";
            return await _dataAccess.GetById<Manufacturer>(sql, new { ManufacturerID = manufacturerId });
        }

        public async Task<List<Product>> GetProductsByNameAsync(string name)
        {
            var query = @"
        SELECT TOP 5 * 
        FROM Product 
        WHERE ProductName LIKE @Name + '%' 
        ORDER BY ProductName";
            var parameters = new { Name = name };
            return await _dataAccess.GetAll<Product>(query, parameters);
        }




    }
}
