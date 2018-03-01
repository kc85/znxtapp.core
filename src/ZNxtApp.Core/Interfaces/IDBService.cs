using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IDBService
    {
        string Collection { get; set; }

        bool WriteData(JObject data);

        long GetCount(string bsonQuery);

        JArray Get(string bsonQuery, List<string> properties = null, Dictionary<string, int> sortColumns = null, int? top = null, int? skip = null);

        long Delete(string bsonQuery);

        long Update(string bsonQuery, JObject data, bool overrideData = false, MergeArrayHandling mergeType = MergeArrayHandling.Union);

        bool DropDB();
    }

    public enum SortBy
    {
        Ascending,
        Descending
    }
}