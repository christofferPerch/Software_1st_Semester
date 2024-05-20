using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class ReviewService {
        private readonly IDataAccess _dataAccess;

        public ReviewService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<Review?> GetReviewByProductID(int productID) {
            string sql = $"SELECT * FROM Review WHERE ProductID = @ProductID";
            return await _dataAccess.GetById<Review>(sql, new { ProductID = productID });
        }
    }
}
