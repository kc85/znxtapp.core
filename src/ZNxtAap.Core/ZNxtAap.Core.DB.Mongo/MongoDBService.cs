using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Exceptions;
using ZNxtAap.Core.Exceptions.ErrorCodes;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.DB.Mongo
{
    public class MongoDBService : IDBService
    {
        private IMongoDatabase _mongoDataBase;
        private MongoClient _mongoClient;

        private string _collection;

        public string Collection
        {
            get
            {
                if (string.IsNullOrEmpty(_collection.Trim()))
                {
                    throw new InvalidOperationException("Collection canont be empty");
                }
                return _collection;
            }
            set { _collection = value; }
        }

        private string _dbName;

        public MongoDBService(string dbName)
        {
            _dbName = dbName;
            Init();
        }

        public MongoDBService(string dbName, string collection)
        {
            _dbName = dbName;
            _collection = collection;
            Init();
        }

        public bool WriteData(Newtonsoft.Json.Linq.JObject data)
        {
            try
            {
                UpdateID(data);
                var dbcollection = _mongoDataBase.GetCollection<BsonDocument>(Collection);
                MongoDB.Bson.BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(data.ToString());
                dbcollection.InsertOne(document);
                return true;
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("duplicate key error"))
                {
                    throw new DuplicateDBIDException((int)ErrorCode.DB.DUPLICATE_ID, ErrorCode.DB.DUPLICATE_ID.ToString(), ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private void UpdateID(Newtonsoft.Json.Linq.JObject data)
        {
            if (data[CommonConst.CommonField.ID] == null && data[CommonConst.CommonField.DISPLAY_ID] != null)
            {
                data[CommonConst.CommonField.ID] = data[CommonConst.CommonField.DISPLAY_ID];
            }
            else if (data[CommonConst.CommonField.ID] != null && data[CommonConst.CommonField.DISPLAY_ID] == null)
            {
                data[CommonConst.CommonField.DISPLAY_ID] = data[CommonConst.CommonField.ID];
            }
        }

        public JArray Get(string bsonQuery, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null)
        {
            var findOptions = new FindOptions<BsonDocument>();

            GetFilterProperty(properties, top, skip, findOptions);
            JObject sort = JObject.Parse("{}");
            if (sortColumns != null)
            {
                foreach (var col in sortColumns)
                {
                    sort[col.Key] = col.Value;
                }
            }
            findOptions.Sort = sort.ToString();
            return GetData(bsonQuery, findOptions);
        }

        private static void GetFilterProperty(List<string> properties, int? top, int? skip, FindOptions<BsonDocument> findOptions)
        {
            findOptions.Limit = top;
            findOptions.Skip = skip;
            if (properties != null)
            {
                JObject objProjection = JObject.Parse("{}");

                foreach (var item in properties)
                {
                    objProjection[item] = 1;
                }
                findOptions.Projection = objProjection.ToString();
            }
        }

        private JArray GetData(string bsonQuery, FindOptions<BsonDocument> findOptions)
        {
            IMongoCollection<BsonDocument> query = _mongoDataBase.GetCollection<BsonDocument>(Collection);
            JArray resultData = new JArray();

            using (var cursor = query.FindAsync<BsonDocument>(GetFilter(bsonQuery), findOptions).Result)
            {
                while (cursor.MoveNext())
                {
                    var batch = cursor.Current;
                    foreach (BsonDocument document in batch)
                    {
                        var documentJson = MongoDB.Bson.BsonExtensionMethods.ToJson(document, new MongoDB.Bson.IO.JsonWriterSettings { OutputMode = MongoDB.Bson.IO.JsonOutputMode.Strict });
                        var jobjData = JObject.Parse(documentJson);
                        jobjData.Remove(CommonConst.CommonField.ID);
                        resultData.Add(jobjData);
                    }
                }
            }
            return resultData;
        }

        public long Delete(string bsonQuery)
        {
            var result = _mongoDataBase.GetCollection<BsonDocument>(Collection).DeleteMany(GetFilter(bsonQuery));
            return result.DeletedCount;
        }

        public long Update(string bsonQuery, Newtonsoft.Json.Linq.JObject data, bool overrideData = false, MergeArrayHandling mergeType = MergeArrayHandling.Union)
        {
            var dataResut = Get(bsonQuery, null, null);
            var dbcollection = _mongoDataBase.GetCollection<BsonDocument>(Collection);
            if (overrideData)
            {
                if (dataResut.Count > 1)
                {
                    throw new InvalidFilterException((int)ErrorCode.DB.MULTIPLE_ROW_RETURNED, string.Format("Update replace command cannot execute in multiple rows"));
                }
                if (dataResut.Count == 1)
                {
                    (dataResut[0] as JObject).Merge(data, new JsonMergeSettings
                    {
                        MergeArrayHandling = mergeType
                    });

                    if (data[CommonConst.CommonField.ID] != null)
                    {
                        dataResut[0][CommonConst.CommonField.ID] = data[CommonConst.CommonField.ID];
                    }
                    MongoDB.Bson.BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataResut[0].ToString());
                    ReplaceOneResult result = dbcollection.ReplaceOne(GetFilter(bsonQuery), document, new UpdateOptions() { IsUpsert = true });
                    if (dataResut.Count != result.ModifiedCount)
                    {
                        throw new ClientValidationError((int)ErrorCode.DB.UPDATE_DATA_COUNT_NOT_MATCH, ErrorCode.DB.UPDATE_DATA_COUNT_NOT_MATCH.ToString(), null);
                    }
                }
                else
                {
                    WriteData(data);
                }
            }
            else
            {
                foreach (var item in dataResut)
                {
                    (item as JObject).Merge(data, new JsonMergeSettings
                    {
                        MergeArrayHandling = mergeType
                    });
                }
                var collection = _mongoDataBase.GetCollection<BsonDocument>(Collection);
                foreach (var item in dataResut)
                {
                    if (data[CommonConst.CommonField.ID] != null)
                    {
                        item[CommonConst.CommonField.ID] = data[CommonConst.CommonField.ID];
                    }
                    MongoDB.Bson.BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(item.ToString());
                    string filter = "{" + CommonConst.CommonField.DISPLAY_ID + " : '" + item[CommonConst.CommonField.DISPLAY_ID].ToString() + "'}";
                    ReplaceOneResult result = dbcollection.ReplaceOne(GetFilter(filter), document, new UpdateOptions() { IsUpsert = false });
                }
            }
            return dataResut.Count;
        }

        private void Init()
        {
            _mongoClient = new MongoClient(ApplicationConfig.MongoDBConnectionString);
            _mongoDataBase = _mongoClient.GetDatabase(_dbName);
        }

        private FilterDefinition<BsonDocument> GetFilter(string query)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Empty;
            filter &= MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(query);
            return query;
        }

        public long GetCount(string bsonQuery)
        {
            IMongoCollection<BsonDocument> query = _mongoDataBase.GetCollection<BsonDocument>(Collection);
            var result = query.Find<BsonDocument>(GetFilter(bsonQuery)).Count();
            return result;
        }

        public bool DropDB()
        {
            try
            {
                _mongoClient.DropDatabase(_dbName);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}