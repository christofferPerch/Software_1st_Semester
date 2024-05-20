using HeartDisease.DataAccess;
using HeartDisease.Models;

namespace HeartDisease.Services
{
    public class DatabaseManagementService
    {
        private readonly IDataAccess _dataAccess;

        public DatabaseManagementService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<int> CheckIndexFragmentation(DateTime checkDate)
        {
            var parameters = new { CheckDate = checkDate };
            return await _dataAccess.ExecuteStoredProcedure<int>("SP_CheckIndexFragmentation", parameters);
        }

        public async Task<List<FragmentationHistory>> GetLatestIndexFragmentation()
        {
            return await _dataAccess.ExecuteStoredProcedureList<FragmentationHistory>("SP_LatestIndexFragmentation", new { });
        }

        public async Task<int> RebuildIndex(string indexName)
        {
            var parameters = new { IndexName = indexName };
            return await _dataAccess.ExecuteStoredProcedure<int>("SP_RebuildIndex", parameters);
        }

        public async Task<int> ReorganizeIndex(string indexName)
        {
            var parameters = new { IndexName = indexName };
            return await _dataAccess.ExecuteStoredProcedure<int>("SP_ReorganizeIndex", parameters);
        }
    }
}
