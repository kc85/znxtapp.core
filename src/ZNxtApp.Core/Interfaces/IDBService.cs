using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Interfaces
{
    public interface IDBService
    {
        //bool WriteData(string collection, JObject data);

        [Obsolete("Use method GetCount(string collection, FilterQuery filters)")]
        long GetCount(string collection, string bsonQuery);

        [Obsolete("Use method Get(string collection, DBQuery query, int? top = null, int? skip = null)")]
        JArray Get(string collection, string bsonQuery, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null);

        [Obsolete("Use method Delete(string collection, FilterQuery filters)")]
        long Delete(string collection, string bsonQuery);

        [Obsolete("Use method Update(string collection, FilterQuery filters, JObject data, bool overrideData = false, bool validateSchma = true, MergeArrayHandling mergeType = MergeArrayHandling.Union)")]
        long Update(string collection, string bsonQuery, JObject data, bool overrideData = false, MergeArrayHandling mergeType = MergeArrayHandling.Union);

        [Obsolete("Use method GetPageData(string collection, IDBQueryBuilder query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1)")]
        JObject GetPageData(string collection, string query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1);

        bool DropDB();

        bool WriteData(string collection, JObject data, bool validateSchema = false);

        bool WriteData(string collection, JObject data, string schema);

        bool PutSchema(string collection, string schema);

        string GetSchema(string collection);

        long GetCount(string collection, FilterQuery filters);

        long GetCount(string collection, IDBQueryBuilder query);

        JArray Get(string collection, DBQuery query, int? top = null, int? skip = null);

        JArray Get(string collection, IDBQueryBuilder query, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null);

        long Delete(string collection, FilterQuery filters);

        long Delete(string collection, IDBQueryBuilder query);

        long Update(string collection, FilterQuery filters, JObject data, bool overrideData = false, bool validateSchma = false, MergeArrayHandling mergeType = MergeArrayHandling.Union);

        JObject GetPageData(string collection, IDBQueryBuilder query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1);


    }

    [Obsolete("Use SortType")]
    public enum SortBy
    {
        Ascending,
        Descending
    }
}