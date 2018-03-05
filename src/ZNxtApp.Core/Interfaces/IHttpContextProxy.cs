using Newtonsoft.Json.Linq;
using System;

namespace ZNxtApp.Core.Interfaces
{
    public interface IHttpContextProxy : IInitData
    {
     
        string GetURIAbsolutePath();

        string GetHttpMethod();

        int ResponseStatusCode { get; }
        string ResponseStatusMessage { get; }
        byte[] Response { get; }

        void SetResponse(int statusCode, JObject data = null);

        void SetResponse(int statusCode, string data);

        void SetResponse(int statusCode, byte[] data);

        void SetResponse(string data);

        void SetResponse(byte[] data);

        string ContentType { get; set; }

        string GetMimeType(string fileName);

        string GetRequestBody();

        T GetRequestBody<T>();

        string GetQueryString(string key);

        string GetFormData(string key);

        string SessionID{get;}

        //dynamic GetFiles();

        //string GetFormData(string key);

        //string GetHttpMethod(string key);

        //string GetMimeMapping(string fileName);

        //string GetQueryString(string key);

        //string GetRequestBody();

        //Newtonsoft.Json.Linq.JObject GetSessionValue(string key);

        //int HttpResponseStatusCode { get; set; }
        //string HttpResponseStatusMessage { get; set; }

        //void ResetSession();

        //void SetSessionValue(string key, Newtonsoft.Json.Linq.JObject value);

        //void UploadAppDomain();

        //RoutingModel GetRoute();

        //RoutingModel GetRoute(string method, string path);

        string GetContentType(string path);

        //List<string> GetSessionUserGroups();

        //bool HasAccess(JObject data);

        //JObject GetSessionUser();

        //string ServeMapPath(string path);
    }
}