using HeartDisease.DataAccess;
using HeartDisease.Models;

namespace HeartDisease.Services {
    public class DatabaseManagementService {
        private readonly IDataAccess _dataAccess;

        public DatabaseManagementService(IDataAccess dataAccess) {
            _dataAccess = dataAccess;
        }
        public async Task<List<FragmentationInfo>> CheckFragmentationAsync() {
            var sql = "CheckIndexFragmentation"; // Stored procedure name
            var result = await _dataAccess.ExecuteStoredProcedure<List<FragmentationInfo>>(sql, new { });
            return result?.ToList() ?? new List<FragmentationInfo>();
        }


        public async Task RebuildIndexesAsync(string tableName) {
            var sql = $"ALTER INDEX ALL ON {tableName} REBUILD";
            await _dataAccess.ExecuteScalar<int>(sql, new { });
        }

        public async Task ReorganizeIndexesAsync(string tableName) {
            var sql = $"ALTER INDEX ALL ON {tableName} REORGANIZE";
            await _dataAccess.ExecuteScalar<int>(sql, new { });
        }
    }
}
