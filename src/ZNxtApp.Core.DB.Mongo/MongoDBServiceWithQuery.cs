using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Exceptions;
using ZNxtApp.Core.Exceptions.ErrorCodes;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using static ZNxtApp.Core.Exceptions.ErrorCodes.ErrorCode;

namespace ZNxtApp.Core.DB.Mongo
{
    public partial class MongoDBService : IDBService
    {
        private const string COLLECTION_SCHEMA = "json_schema";
        public string DUPLICATE_KEY_ERROR { get; private set; }

        public bool WriteData(string collection, JObject data, bool validateSchema = false)
        {
            var schema = string.Empty;
            if (validateSchema)
            {
                schema = GetSchema(collection);
            }
            return WriteData(collection, data, schema);
        }

        public bool WriteData(string collection, JObject data, string schema)
        {
            IList<string> errorMessages = new List<string>();
            if (string.IsNullOrEmpty(schema) || _JSONValidator.Validate(schema, data, out errorMessages))
            {
                try
                {
                    UpdateCommonData(data);
                    data[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
                    data[CommonConst.CommonField.UPDATED_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
                    data[CommonConst.CommonField.CREATED_BY] = GetUserId();
                    var dbcollection = _mongoDataBase.GetCollection<BsonDocument>(collection);
                    BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(data.ToString());
                    dbcollection.InsertOne(document);
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(DUPLICATE_KEY_ERROR))
                    {
                        throw new DuplicateDBIDException((int)ErrorCode.DB.DUPLICATE_ID, ErrorCode.DB.DUPLICATE_ID.ToString(), ex);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                throw new SchemaValidationException((int)DBSchemaValidation.DB_SCHEMA_VALIDATION_ERROR, errorMessages);
            }
        }

        public bool PutSchema(string collection, string schema)
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery() {
                new Filter(CommonConst.CommonField.DISPLAY_ID,GetCollectionSchemaId(collection))
            }
            };
            JObject data = new JObject
            {
                [CommonConst.CommonField.DISPLAY_ID] = GetCollectionSchemaId(collection),
                [CommonConst.CommonField.SCHEMA] = schema
            };
            return Update(COLLECTION_SCHEMA, query.Filters, data, true, false, MergeArrayHandling.Replace) == 1 ? true : false;
        }

        private string GetCollectionSchemaId(string collection)
        {
            return string.Format("$$_$_{0}", collection);
        }

        public string GetSchema(string collection)
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery() {
                new Filter(CommonConst.CommonField.DISPLAY_ID,GetCollectionSchemaId(collection))
            }
            };
            var dataResult = Get(COLLECTION_SCHEMA, query);
            if (dataResult.Count != 1)
            {
                throw new Exception("Schema not found");
            }
            return dataResult.First()[CommonConst.CommonField.SCHEMA].ToString();
        }

        public long GetCount(string collection, FilterQuery filters)
        {
            return GetCount(collection, new MongoQueryBuilder(filters));
        }

        public long GetCount(string collection, IDBQueryBuilder query)
        {
            IMongoCollection<BsonDocument> queryFilter = _mongoDataBase.GetCollection<BsonDocument>(collection);
            long result = queryFilter.Find(GetFilter(query.GetQuery())).Count();
            return result;
        }

        public JArray Get(string collection, DBQuery query, int? top = default(int?), int? skip = default(int?))
        {
            return Get(collection, new MongoQueryBuilder(query.Filters), GetProperties(query), GetSortBy(query), top, skip);
        }

        public JArray Get(string collection, IDBQueryBuilder query, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = default(int?), int? skip = default(int?))
        {
            var findOptions = new FindOptions<BsonDocument>();

            GetFilterProperty(properties, top, skip, findOptions);
            JObject sort = new JObject();
            if (sortColumns != null)
            {
                foreach (var col in sortColumns)
                {
                    sort[col.Key] = col.Value;
                }
            }
            findOptions.Sort = sort.ToString();
            return GetData(collection, query.GetQuery(), findOptions);
        }

        private List<string> GetProperties(DBQuery query)
        {
            List<string> fields = new List<string>();
            foreach (var field in query.Fields)
            {
                fields.Add(field.Name);
            }
            return fields;
        }

        private Dictionary<string, int> GetSortBy(DBQuery query)
        {
            Dictionary<string, int> sort = new Dictionary<string, int>();
            foreach (var sortBy in query.SortBy)
            {
                sort[sortBy.Name] = (int)sortBy.Sort;
            }
            return sort;
        }

        public long Delete(string collection, FilterQuery filters)
        {
            return Delete(collection, new MongoQueryBuilder(filters));
        }

        public long Delete(string collection, IDBQueryBuilder query)
        {
            var result = _mongoDataBase.GetCollection<BsonDocument>(collection).DeleteMany(query.GetQuery());
            return result.DeletedCount;
        }

        public long Update(string collection, FilterQuery filters, JObject data, bool overrideData = false, bool validateSchma = false, MergeArrayHandling mergeType = MergeArrayHandling.Union)
        {
            return Update(collection, new MongoQueryBuilder(filters), data, overrideData, validateSchma, mergeType);
        }

        public long Update(string collection, IDBQueryBuilder query, JObject data, bool overrideData = false, bool validateSchma = false, MergeArrayHandling mergeType = MergeArrayHandling.Union)
        {
            data[CommonConst.CommonField.UPDATED_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
            data[CommonConst.CommonField.UPDATED_BY] = GetUserId();
            var dataResut = Get(collection, query, null, null);
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
                    BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(dataResut[0].ToString());
                    ReplaceOneResult result = dbcollection.ReplaceOne(GetFilter(query.GetQuery()), document, new UpdateOptions() { IsUpsert = true });
                    if (dataResut.Count != result.ModifiedCount)
                    {
                        throw new ClientValidationError((int)ErrorCode.DB.UPDATE_DATA_COUNT_NOT_MATCH, ErrorCode.DB.UPDATE_DATA_COUNT_NOT_MATCH.ToString(), null);
                    }
                }
                else
                {
                    WriteData(collection, data, validateSchma);
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
                    BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(item.ToString());

                    // todo
                    string filter = "{" + CommonConst.CommonField.DISPLAY_ID + " : '" + item[CommonConst.CommonField.DISPLAY_ID].ToString() + "'}";
                    ReplaceOneResult result = dbcollection.ReplaceOne(GetFilter(filter), document, new UpdateOptions() { IsUpsert = false });
                }
            }
            return dataResut.Count;
        }

        public JObject GetPageData(string collection, IDBQueryBuilder query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1)
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
    }
}