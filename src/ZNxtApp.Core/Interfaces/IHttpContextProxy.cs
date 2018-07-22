using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IHttpContextProxy : IInitData
    {
     
        string GetURIAbsolutePath();

        string GetHttpMethod();
        Dictionary<string, string> ResponseHeaders { get; set; }

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

        string GetContentType(FileInfo pathInfo);

        string GetRequestBody();

        T GetRequestBody<T>();

        string GetQueryString(string key);

        string GetFormData(string key);

        string SessionID{get;}

        void UnloadAppDomain();
        string GetHeader(string key);
        Dictionary<string,string> GetHeaders();
        string GetContentType(string path);
        
    }
}