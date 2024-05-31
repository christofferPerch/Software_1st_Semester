using HeartDisease.DataAccess;
using HeartDisease.Models.Webshop;

namespace HeartDisease.Services.Webshop {
    public class SideEffectService {
        private readonly IDataAccess _dataAccess;

        public SideEffectService(IDataAccess dataAccess) {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

public async Task<SideEffect?> GetSideEffectByProductID(int productID) {
            string sql = "SELECT * FROM SideEffect WHERE ProductID = @ProductID";
            return await _dataAccess.GetById<SideEffect>(sql, new { ProductID = productID });
        }

    }
}
