using HeartDisease.DataAccess;
using HeartDisease.Models;
using HeartDisease.Utils;
using Dapper;
using System.Data;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HeartDisease.Services
{
    public class KnowledgeBaseService
    {
        private readonly IMongoDataAccess _mongoDataAccess;

        public KnowledgeBaseService(IMongoDataAccess mongoDataAccess)
        {
            _mongoDataAccess = mongoDataAccess ?? throw new ArgumentNullException(nameof(mongoDataAccess));
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File is empty", nameof(fileStream));

            return await _mongoDataAccess.UploadFileAsync(fileName, fileStream);
        }
        public async Task<List<GridFSFileInfo>> GetAllFilesAsync()
        {
            return await _mongoDataAccess.GetAllFilesAsync();
        }

        public async Task DeleteFileByIdAsync(string fileId)
        {
            await _mongoDataAccess.DeleteFileByIdAsync(new ObjectId(fileId));
        }

        public async Task DeleteFileByNameAsync(string filename)
        {
            await _mongoDataAccess.DeleteFileByNameAsync(filename);
        }
        public async Task<(Stream fileStream, string fileName)> GetFileAsync(ObjectId fileId)
        {
            return await _mongoDataAccess.GetFileAsync(fileId); 
        }
    }
}
