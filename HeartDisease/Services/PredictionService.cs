using HeartDisease.DataAccess;
using HeartDisease.Models;
using Dapper;
using System.Data;

namespace HeartDisease.Services {
    public class PredictionService {

        private readonly IDataAccess _dataAccess;

        public PredictionService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public async Task<List<PredictionModel>> GetAllPredictionParamters()
        {
            string SqlStr = @"SELECT TOP (10) *
                             FROM HearthSurvey";

            return await _dataAccess.GetAll<PredictionModel>(SqlStr);
        }
    }
}
