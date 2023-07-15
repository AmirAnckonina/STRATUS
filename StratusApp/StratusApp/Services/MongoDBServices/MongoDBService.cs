using StratusApp.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;
using System.Linq.Expressions;

namespace StratusApp.Services.MongoDBServices
{
    public class MongoDBService
    {
        private readonly MongoClient _mongoClient;
        private Dictionary<string, List<string>> _databasesAndCollections;
        public MongoDBService(MyDatabaseSettings mongoDbSettings) 
        {
            _mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        }
        public async Task<Dictionary<string, List<string>>> GetDatabasesAndCollections()
        {
            if (_databasesAndCollections != null) return _databasesAndCollections;

            _databasesAndCollections = new Dictionary<string, List<string>>();
            var databasesResult = _mongoClient.ListDatabaseNames();

            await databasesResult.ForEachAsync(async databaseName =>
            {
                var collectionNames = new List<string>();
                var database = _mongoClient.GetDatabase(databaseName);
                var collectionNamesResult = database.ListCollectionNames();
                await collectionNamesResult.ForEachAsync(
                    collectionName => { collectionNames.Add(collectionName); });
                _databasesAndCollections.Add(databaseName, collectionNames);
            });

            return _databasesAndCollections;
        }
        public async Task<List<BsonDocument>> GetDocumentsByFilter(string databaseName, string collectionName, Expression<Func<BsonDocument, bool>> filter)
        {
            var collection = GetCollection(databaseName, collectionName);
            var bson = new BsonDocument();
            try
            {
                var documents = await collection.Find(filter).ToListAsync();
                return documents;
            }
            catch (Exception)
            {
                throw;
            }   
            
        }

        public async Task<long> GetCollectionCount(string databaseName, string collectionName)
        {
            var collection = GetCollection(databaseName, collectionName);
            return await collection.EstimatedDocumentCountAsync();
        }

        private IMongoCollection<BsonDocument> GetCollection(string databaseName, string collectionName)
        {
            var db = _mongoClient.GetDatabase(databaseName);
            return db.GetCollection<BsonDocument>(collectionName);
        }
        public async Task<UpdateResult> CreateOrUpdateField(string databaseName, string collectionName, string id, string fieldName, string value)
        {
            var collection = GetCollection(databaseName, collectionName);
            var update = Builders<BsonDocument>.Update.Set(fieldName, new BsonString(value));
            return await collection.UpdateOneAsync(CreateIdFilter(id), update);
        }

        public async Task<DeleteResult> DeleteDocument(string databaseName, string collectionName, string id)
        {
            var collection = GetCollection(databaseName, collectionName);
            return await collection.DeleteOneAsync(CreateIdFilter(id));
        }

        private static BsonDocument CreateIdFilter(string id)
        {
            return new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
        }

        public async Task InsertDocument(string databaseName, string collectionName)
        {
            var collection = GetCollection(databaseName, collectionName);
            var insert = new BsonDocument()
            .Set("email", "bobo")
            .Set("password", "1111")
            .Set("accessKey", "1111")
            .Set("secretKey", "1111");
            await collection.InsertOneAsync(insert);
        }

    }
}
