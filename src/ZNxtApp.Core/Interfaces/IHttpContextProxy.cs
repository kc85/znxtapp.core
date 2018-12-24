using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace ZNxtApp.Core.Interfaces
{
    public interface IHttpContextProxy : IInitData
    {
        /// <summary>
        /// Get Request Absoulate Path
        /// </summary>
        /// <returns></returns>
        string GetURIAbsolutePath();

        /// <summary>
        /// Get HTTP method. e.g : GET, POST, DELETE etc.
        /// </summary>
        /// <returns></returns>
        string GetHttpMethod();

        /// <summary>
        /// Response Headers
        /// </summary>
        Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Get the HTTP response status code
        /// </summary>
        int ResponseStatusCode { get; }

        /// <summary>
        /// Get string HTTP response message
        /// </summary>
        string ResponseStatusMessage { get; }

        /// <summary>
        /// Get byte [] HTTP response message
        /// </summary>
        byte[] Response { get; }

        /// <summary>
        /// Set HTTP Response with status code and JSON object
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        void SetResponse(int statusCode, JObject data = null);

        /// <summary>
        /// Set HTTP response with status code and string data
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        void SetResponse(int statusCode, string data);

        /// <summary>
        /// Set HTTP response  with status code and byte []
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="data"></param>
        void SetResponse(int statusCode, byte[] data);

        /// <summary>
        /// Set Response with string data
        /// </summary>
        /// <param name="data"></param>
        void SetResponse(string data);

        /// <summary>
        /// Set byte [] response
        /// </summary>
        /// <param name="data"></param>
        void SetResponse(byte[] data);

        /// <summary>
        /// Set and Get HTTP response code.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Get Mime Type from file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetMimeType(string fileName);

        /// <summary>
        /// Get Content type from file info
        /// </summary>
        /// <param name="pathInfo"></param>
        /// <returns></returns>
        string GetContentType(FileInfo pathInfo);

        /// <summary>
        /// Get HTTP request body.
        /// </summary>
        /// <returns></returns>
        string GetRequestBody();

        /// <summary>
        /// Get HTTP request with type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetRequestBody<T>();

        /// <summary>
        /// Get HTTP request query string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetQueryString(string key);

        /// <summary>
        /// Get HTTP form data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetFormData(string key);

        /// <summary>
        /// Get Session ID
        /// </summary>
        string SessionID { get; }

        /// <summary>
        /// Unload App Domain
        /// </summary>
        void UnloadAppDomain();

        /// <summary>
        /// Get HTTP headers by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetHeader(string key);

        /// <summary>
        /// Get all HTTP headers
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetHeaders();

        /// <summary>
        /// Get Content type from file Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetContentType(string path);

        /// <summary>
        /// Get temp data from http request scope.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetTempValue<T>(string key);

        /// <summary>
        /// Set temp data available for the request scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetTempValue<T>(string key, T value);
    }
}