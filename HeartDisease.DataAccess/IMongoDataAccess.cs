﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;

namespace HeartDisease.DataAccess
{
    public interface IMongoDataAccess
    {
        Task<T?> GetById<T>(string id);
        Task<List<T>> GetAll<T>();
        Task Insert<T>(T entity);
        Task Update<T>(string id, T entity);
        Task Delete<T>(string id);
        Task<string> UploadFileAsync(string fileName, Stream fileStream);
        Task<(Stream fileStream, string fileName)> GetFileAsync(ObjectId fileId);
        Task<List<GridFSFileInfo>> GetAllFilesAsync();
        Task DeleteFileByIdAsync(ObjectId fileId);
        Task DeleteFileByNameAsync(string filename);

    }
}
