using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IHttpFileUploader
    {
        List<string> GetFiles();
        string Save(string fileName, string destination = null);
        JObject SaveToDB(IDBService dbProxy, string fileName, string baseFolder, string collection, string updateFilter = null);
    }
}
