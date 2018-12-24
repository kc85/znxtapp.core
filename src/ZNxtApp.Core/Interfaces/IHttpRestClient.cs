using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Interfaces
{
    public interface IHttpRestClient
    {
        T Call<T>(HttpRestRequest request) where T : new();
        JObject Get(string url, Dictionary<string, string> headres = null, int ? timeout = null);
        JObject Post(string url, JObject postData, Dictionary<string,string> headres = null, int? timeout = null);
        JObject Put(string url, JObject postData, Dictionary<string, string> headres = null, int? timeout = null);
        JObject Delete(string url, JObject postData, Dictionary<string, string> headres = null, int? timeout = null);
    }
    public class HttpRestRequest
    {
        public string Method { get; set; }
        public string URL { get; set; }
        public object Body { get; set; }
        public int Timeout { get; set; }
        public Dictionary<string, string> Headres { get; set; }
        public HttpRestRequest(string url,int ? timeout = null)
        {
            this.URL = url;
            Method = CommonConst.ActionMethods.GET;
            if (timeout == null)
            {
                Timeout = 1000*30;
            }
            else
            {
                Timeout = timeout.Value;
            }
        }
    }
}
