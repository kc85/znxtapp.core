using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IDBService
    {
        bool WriteData(string collection, JObject data);

        long GetCount(string collection, string bsonQuery);

        JArray Get(string collection, string bsonQuery, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null);

        long Delete(string collection, string bsonQuery);

        long Update(string collection, string bsonQuery, JObject data, bool overrideData = false, MergeArrayHandling mergeType = MergeArrayHandling.Union);

        JObject GetPageData(string collection, string query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1);

        bool DropDB();
    }

    public enum SortBy
    {
        Ascending,
        Descending
    }
}