﻿namespace HeartDisease.DataAccess {
    public interface IDataAccess {

        Task<T?> GetById<T>(string sql, object parameters);
        Task<List<T>> GetAll<T>(string sql, object? parameters = null);

        Task<int> Insert(string sql, object parameters);
        Task<int> Update(string sql, object parameters);
        Task<int> Delete(string sql, object parameters);
        Task<T?> ExecuteStoredProcedure<T>(string procedureName, object parameters);
        Task<T> ExecuteScalar<T>(string sql, object? parameters = null);
        //Task<int> InsertAndReturnId(string sql, object parameters);


    }
}
