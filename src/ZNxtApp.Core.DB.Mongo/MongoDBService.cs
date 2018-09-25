using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Exceptions;
using ZNxtApp.Core.Exceptions.ErrorCodes;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.DB.Mongo
{
    public class MongoDBService : IDBService
    {
        private IMongoDatabase _mongoDataBase;
        private MongoClient _mongoClient;
        public Func<string> User;

        private string _dbName;

        public MongoDBService(string dbName)
        {
            _dbName = dbName;
            Init();
        }

        private string GetUserId()
        {
            return User != null ? User() : "";
        }

        public bool WriteData(string collection, Newtonsoft.Json.Linq.JObject data)
        {
            try
            {
                UpdateCommonData(data);
                data[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
                data[CommonConst.CommonField.UPDATED_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
                data[CommonConst.CommonField.CREATED_BY] = GetUserId();
                var dbcollection = _mongoDataBase.GetCollection<BsonDocument>(collection);
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

        private void UpdateCommonData(Newtonsoft.Json.Linq.JObject data)
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

        public JArray Get(string collection, string bsonQuery, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null)
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
            return GetData(collection, bsonQuery, findOptions);
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

        private JArray GetData(string collection, string bsonQuery, FindOptions<BsonDocument> findOptions)
        {
            IMongoCollection<BsonDocument> query = _mongoDataBase.GetCollection<BsonDocument>(collection);
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

        public JObject GetPageData(string collection, string query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1)
        {
            int? top = null;
            int? skip = null;

            top = pageSize;
            skip = (pageSize * (currentPage - 1));

            var dbArrData = Get(collection, query, fields, sortColumns, top, skip);
            JObject extraData = new JObject();
            long count = GetCount(collection, query);

            extraData[CommonConst.CommonField.TOTAL_RECORD_COUNT_KEY] = count;
            extraData[CommonConst.CommonField.TOTAL_PAGES_KEY] = Math.Ceiling(((double)count / pageSize));
            extraData[CommonConst.CommonField.PAGE_SIZE_KEY] = pageSize;
            extraData[CommonConst.CommonField.CURRENT_PAGE_KEY] = currentPage;
            extraData[CommonConst.CommonField.DATA] = dbArrData;

            return extraData;
        }

        public long Delete(string collection, string bsonQuery)
        {
            var result = _mongoDataBase.GetCollection<BsonDocument>(collection).DeleteMany(GetFilter(bsonQuery));
            return result.DeletedCount;
        }

        public long Update(string collection, string bsonQuery, Newtonsoft.Json.Linq.JObject data, bool overrideData = false, MergeArrayHandling mergeType = MergeArrayHandling.Union)
        {
            data[CommonConst.CommonField.UPDATED_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
            data[CommonConst.CommonField.UPDATED_BY] = GetUserId();
            var dataResut = Get(collection, bsonQuery, null, null);
            var dbcollection = _mongoDataBase.GetCollection<BsonDocument>(collection);
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
                    WriteData(collection, data);
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

        public long GetCount(string collection, string bsonQuery)
        {
            IMongoCollection<BsonDocument> query = _mongoDataBase.GetCollection<BsonDocument>(collection);
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