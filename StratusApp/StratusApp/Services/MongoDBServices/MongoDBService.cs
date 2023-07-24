using StratusApp.Models.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SecurityToken.Model;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using Utils.DTO;
using DTO;
using OpenQA.Selenium;

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

            RegisterToMapClasses();
         }

        private void RegisterToMapClasses()
        {
            var res = BsonClassMap.RegisterClassMap<AlertData>(classMap =>
            {
                classMap.AutoMap();
            });
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

        public async Task<List<BsonDocument>> GetDocuments(eCollectionName collectionType, Func<BsonDocument, bool> filter = null)
        {
            List<BsonDocument> result = new List<BsonDocument>();
            List<BsonDocument> collection = await GetCollectionAsList<BsonDocument>(collectionType); 
            
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

        public async Task<long> GetCollectionCount<T>(eCollectionName collectionType)
        {
            var collection = GetCollection<T>(collectionType);

            return await collection.EstimatedDocumentCountAsync();
        }

        public IMongoCollection<T> GetCollection<T>(eCollectionName collectionType)
        {
            var db = _mongoClient.GetDatabase(DB_NAME);
            var colName = MongoDBCollectionNames.GetCollectionName(collectionType);

            return db.GetCollection<T>(colName);
        }

        public async Task<List<T>> GetCollectionAsList<T>(eCollectionName collectionType)
        {
            var db = _mongoClient.GetDatabase(DB_NAME);
            var colName = MongoDBCollectionNames.GetCollectionName(collectionType);
            List<T> result = new List<T>();

            var collecition =  await db.GetCollection<BsonDocument>(colName).Find(_ => true).ToListAsync();

            foreach (var item in collecition)
            {
                result.Add(BsonSerializer.Deserialize<T>(item));
            }

            return result;
         }

        public async Task<UpdateResult> CreateOrUpdateField<T>(eCollectionName collectionType, ObjectId id, string fieldName, string value)
        {
            var documentToUpdate = GetDocumentById(collectionType, id);
            var collection = GetCollection<T>(collectionType);
            var update = Builders<T>.Update.Set(fieldName, new BsonString(value));

            return await collection.UpdateOneAsync(documentToUpdate, update);
        }

        public async Task<DeleteResult> DeleteDocument<T>(eCollectionName collectionType, ObjectId id)
        {
            var collection = GetCollection<T>(collectionType);
            var documentToDelete = GetDocumentById(collectionType, id);

            return await collection.DeleteOneAsync(documentToDelete);
        }

        public BsonDocument? GetDocumentById(eCollectionName collectionType, ObjectId id)
        {
            return GetDocuments(collectionType, (document) => document.GetValue("_id") == id).Result.FirstOrDefault();
        }

        private static BsonDocument CreateIdFilter(string id)
        {
            return new BsonDocument("_id", new BsonObjectId(new ObjectId(id)));
        }

        public async Task InsertDocument<T>(eCollectionName collectionType, T documentToInsert)
        {
            var collection = GetCollection<T>(collectionType);

            await collection.InsertOneAsync(documentToInsert);
        }

        public async Task InsertMultipleDocuments<T>(eCollectionName collectionType, List<T> documentsToInsert)
        {
            var collection = GetCollection<T>(collectionType);

            await collection.InsertManyAsync(documentsToInsert);
        }

        public async Task InsertDocumentByForeignKey<T>(eCollectionName collectionType, T documentToInsert, eCollectionName foreignCollectionType, ObjectId foreignKey)
        {
            var collection = GetCollection<T>(collectionType);
            
            if (GetDocumentById(foreignCollectionType, foreignKey) != null)
            {
                //documentToInsert.Set("userId", foreignKey); todo: get class with foreign key and no need to set
                await collection.InsertOneAsync(documentToInsert);
            }
        }

        internal async Task<DeleteResult> DeleteDocuments<T>(eCollectionName collectionType, FilterDefinition<T> filter)
        {
            var collection = GetCollection<T>(collectionType);

            return await collection.DeleteManyAsync(filter);
        }
    }
}
