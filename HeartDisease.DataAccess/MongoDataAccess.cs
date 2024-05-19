using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace HeartDisease.DataAccess {
    public class MongoDataAccess : IMongoDataAccess {
        private readonly IMongoDatabase _database;
        private readonly GridFSBucket _gridFS;


        public MongoDataAccess(string connectionString, string databaseName) {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _gridFS = new GridFSBucket(_database);
        }

        private IMongoCollection<T> GetCollection<T>() {
            return _database.GetCollection<T>(typeof(T).Name);
        }

        public async Task<T?> GetById<T>(string id) {
            var collection = GetCollection<T>();
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAll<T>() {
            var collection = GetCollection<T>();
            return await collection.Find(new BsonDocument()).ToListAsync();
        }

        public async Task Insert<T>(T entity) {
            var collection = GetCollection<T>();
            await collection.InsertOneAsync(entity);
        }

        public async Task Update<T>(string id, T entity) {
            var collection = GetCollection<T>();
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await collection.ReplaceOneAsync(filter, entity);
        }

        public async Task Delete<T>(string id) {
            var collection = GetCollection<T>();
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await collection.DeleteOneAsync(filter);
        }
        public async Task<string> UploadFileAsync(string fileName, Stream fileStream) {
            ObjectId objectId = await _gridFS.UploadFromStreamAsync(fileName, fileStream);
            return objectId.ToString();
        }

        public async Task<List<GridFSFileInfo>> GetAllFilesAsync() {
            var files = new List<GridFSFileInfo>();
            using (var cursor = await _gridFS.FindAsync(new BsonDocument())) {
                while (await cursor.MoveNextAsync()) {
                    files.AddRange(cursor.Current);
                }
            }
            return files;
        }

        public async Task DeleteFileByIdAsync(ObjectId fileId) {
            await _gridFS.DeleteAsync(fileId);
        }

        public async Task DeleteFileByNameAsync(string filename) {
            var filter = Builders<GridFSFileInfo>.Filter.Eq(info => info.Filename, filename);
            using (var cursor = await _gridFS.FindAsync(filter)) {
                while (await cursor.MoveNextAsync()) {
                    foreach (var file in cursor.Current) {
                        await _gridFS.DeleteAsync(file.Id);
                    }
                }
            }
        }
        public async Task<(Stream fileStream, string fileName)> GetFileAsync(ObjectId fileId) {
            var fileInfo = await _gridFS.Find(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId)).FirstOrDefaultAsync();
            if (fileInfo == null) {
                return (null, null);
            }

            var fileStream = await _gridFS.OpenDownloadStreamAsync(fileId);
            return (fileStream, fileInfo.Filename); // Make sure to return both stream and filename as a tuple
        }

    }
}
