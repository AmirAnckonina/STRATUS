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
        private const string DB_NAME = "StratusDB";

        public MongoDBService(MyDatabaseSettings mongoDbSettings) 
        {
            _mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        }

        public async Task<Dictionary<string, List<string>>> GetDatabasesAndCollections()
        {
            if (_databasesAndCollections != null) return _databasesAndCollections;

            _databasesAndCollections = new Dictionary<string, List<string>>();
            var databasesResult = _mongoClient.ListDatabaseNames();

            await databasesResult.ForEachAsync(async DB_NAME =>
            {
                var collectionNames = new List<string>();
                var database = _mongoClient.GetDatabase(DB_NAME);
                var collectionNamesResult = database.ListCollectionNames();
                await collectionNamesResult.ForEachAsync(
                    collectionName => { collectionNames.Add(collectionName); });
                _databasesAndCollections.Add(DB_NAME, collectionNames);
            });

            return _databasesAndCollections;
        }

        public async Task<List<BsonDocument>> GetDocuments(string collectionName, Func<BsonDocument, bool> filter = null)
        {
            List<BsonDocument> result = new List<BsonDocument>();
            List<BsonDocument> collection = await GetCollectionAsList(collectionName); 
            
            foreach(BsonDocument document in collection)
            {
                if(filter == null) // no filter added
                {
                    result.Add(document);
                }
                else if(filter(document))
                {
                    result.Add(document);
                }
            }

            return result;
        }

        public async Task<long> GetCollectionCount(string collectionName)
        {
            var collection = GetCollection(collectionName);

            return await collection.EstimatedDocumentCountAsync();
        }

        private IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            var db = _mongoClient.GetDatabase(DB_NAME);

            return db.GetCollection<BsonDocument>(collectionName);
        }

        public async Task<List<BsonDocument>> GetCollectionAsList(string collectionName)
        {
            var db = _mongoClient.GetDatabase(DB_NAME);

            return await db.GetCollection<BsonDocument>(collectionName).Find(_ => true).ToListAsync();
        }

        public async Task<UpdateResult> CreateOrUpdateField(string collectionName, ObjectId id, string fieldName, string value)
        {
            var documentToUpdate = GetDocumentById(collectionName, id);
            var collection = GetCollection(collectionName);
            var update = Builders<BsonDocument>.Update.Set(fieldName, new BsonString(value));

            return await collection.UpdateOneAsync(documentToUpdate, update);
        }

        public async Task<DeleteResult> DeleteDocument(string collectionName, ObjectId id)
        {
            var collection = GetCollection(collectionName);
            var documentToDelete = GetDocumentById(collectionName, id);

            return await collection.DeleteOneAsync(documentToDelete);
        }

        public BsonDocument? GetDocumentById(string collectionName, ObjectId id)
        {
            return GetDocuments(collectionName, (document) => document.GetValue("_id") == id).Result.FirstOrDefault();
        }

        private static BsonDocument CreateIdFilter(string id)
        {
            return new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
        }

        public async Task InsertDocument(string collectionName, BsonDocument documentToInsert = null)
        {
            var collection = GetCollection(collectionName);

            await collection.InsertOneAsync(documentToInsert);
        }

        public async Task InsertDocumentByForeignKey(string collectionName, BsonDocument documentToInsert, string foreignCollectionName, ObjectId foreignKey)
        {
            var collection = GetCollection(collectionName);

            if(GetDocumentById(foreignCollectionName, foreignKey) != null)
            {
                documentToInsert.Set("userId", foreignKey);
                await collection.InsertOneAsync(documentToInsert);
            }
        }
    }
}
