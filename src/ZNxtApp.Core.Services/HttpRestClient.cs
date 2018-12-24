using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Interfaces;
using RestSharp;
using Newtonsoft.Json;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Services
{
    public class HttpRestClient : IHttpRestClient
    {
        public T Call<T>(HttpRestRequest request) where T : new()
        {
            var client = new RestClient();
            client.BaseUrl = new System.Uri(request.URL);
            var restRequest = new RestRequest();
            if (request.Headres != null)
            {
                foreach (var header in request.Headres)
                {
                    restRequest.AddHeader(header.Key, header.Value);
                }
            }
            RestSharp.Method method = Method.GET;
            if (!Enum.TryParse<RestSharp.Method>(request.Method, out method))
            {
                throw new Exception(string.Format("Unsupported HTTP method {0}", request.Method));
            }

            restRequest.Method = method;

            restRequest.Timeout = request.Timeout;

            if (request.Body != null)
            {
                restRequest.AddJsonBody(request.Body);
            }
            var response = client.Execute(restRequest);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                var httpException = new ApplicationException(message, response.ErrorException);
                throw httpException;
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            catch (Exception)
            {
                return default(T);
            }

        }

        public JObject Delete(string url, JObject bodyPayload, Dictionary<string, string> headres = null, int? timeout = null)
        {
            return Call<JObject>(new HttpRestRequest(url,timeout) { Method = CommonConst.ActionMethods.DELETE,  Headres = headres, Body = bodyPayload });
        }

        public JObject Get(string url, Dictionary<string, string> headres = null, int? timeout = null)
        {
            return Call<JObject>(new HttpRestRequest(url, timeout) { Method = CommonConst.ActionMethods.GET, Headres = headres });
        }

        public JObject Post(string url, JObject bodyPayload, Dictionary<string, string> headres = null, int? timeout = null)
        {
            return Call<JObject>(new HttpRestRequest(url, timeout) { Method = CommonConst.ActionMethods.POST, Headres = headres, Body = bodyPayload });
        }

        public JObject Put(string url, JObject bodyPayload, Dictionary<string, string> headres = null, int? timeout = null)
        {
            return Call<JObject>(new HttpRestRequest(url, timeout) { Method = CommonConst.ActionMethods.PUT, Headres = headres, Body = bodyPayload });
        }
    }
}
