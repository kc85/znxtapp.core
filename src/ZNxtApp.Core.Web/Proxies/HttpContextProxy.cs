using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Services;
using System.Collections;
using System.Collections.Generic;

namespace ZNxtApp.Core.Web.Proxies
{
    public partial class HttpContextProxy : IHttpContextProxy
    {
        private HttpContext _context;
        public DateTime InitDateTime { get; private set; }
        private ILogger _logger;
        private DataBuilderHelper _dataBuilderHelper;
        private byte[] _response;

        public byte[] Response
        {
            get { return _response; }
        }
        public string TransactionId { get; private set; }
        private int _responseStatusCode;
        private string _responseStatusMessage;
        public int ResponseStatusCode { get { return _responseStatusCode; } }
        public string ResponseStatusMessage { get { return _responseStatusMessage; } }
        public string ContentType { get; set; }
        private string _requestBody = string.Empty;

        private Dictionary<string, string> _serverPagesContentType = new Dictionary<string, string>();

        public HttpContextProxy(HttpContext context)
        {
            InitDateTime = DateTime.Now;
            _context = context;
            _dataBuilderHelper = new DataBuilderHelper();
            _responseStatusCode = (int)HttpStatusCode.OK;
            _responseStatusMessage = HttpStatusCode.OK.ToString();

            _serverPagesContentType[CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_EXTENSION] = CommonConst.CONTENT_TYPE_TEXT_HTML;
            _serverPagesContentType[CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_TEMPLATE_EXTENSION] = CommonConst.CONTENT_TYPE_TEXT_HTML;
            _serverPagesContentType[CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_BLOCK_EXTENSION] = CommonConst.CONTENT_TYPE_TEXT_HTML;
            _serverPagesContentType[CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_CSS_EXTENSION] = CommonConst.CONTENT_TYPE_TEXT_CSS;
            _serverPagesContentType[CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_JS_EXTENSION] = CommonConst.CONTENT_TYPE_TEXT_JS;

            ContentType = CommonConst.CONTENT_TYPE_TEXT_HTML;

            if (context.Request.Headers[CommonConst.CommonValue.TRANSACTION_ID_KEY] != null)
            {
                TransactionId = context.Request.Headers[CommonConst.CommonValue.TRANSACTION_ID_KEY];
            }
            else
            {
                TransactionId = string.Format("{0}{1}", CommonUtility.GetTimestamp(DateTime.Now), CommonUtility.RandomNumber(2));
            }
            _logger = Logger.GetLogger(this.GetType().FullName,TransactionId);
        }

        public string GetURIAbsolutePath()
        {
            var path = _context.Request.Url.AbsolutePath.ToLower();
            if (!string.IsNullOrEmpty(ApplicationConfig.AppPath) && path.IndexOf(ApplicationConfig.AppPath.ToLower()) == 0)
            {
                path = path.Replace(ApplicationConfig.AppPath.ToLower(), "");
            }
            return path;
        }

        public string GetHttpMethod()
        {
            return _context.Request.HttpMethod;
        }

        public void SetResponse(int statusCode, JObject data = null)
        {
            if (data != null)
            {
                SetResponse(data.ToString());
            }
            else
            {
                var respose = _dataBuilderHelper.GetResponseObject(statusCode);
                if (respose != null)
                {
                    SetResponse(respose.ToString());
                }
            }
        }

        public void SetResponse(int statusCode, string data)
        {
            SetResponse(statusCode);
            SetResponse(data);
        }

        public void SetResponse(int statusCode, byte[] data)
        {
            SetResponse(statusCode);
            SetResponse(data);
        }

        private void SetResponse(int statusCode)
        {
            var code = HttpStatusCode.BadRequest;

            if (Enum.TryParse<HttpStatusCode>(statusCode.ToString(), out code))
            {
                _responseStatusCode = statusCode;
                _responseStatusMessage = code.ToString();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void SetResponse(string data)
        {
            if (data != null)
                _response = Encoding.UTF8.GetBytes(data);
        }

        public void SetResponse(byte[] data)
        {
            _response = data;
        }

        public string GetMimeType(string fileName)
        {
            return GetContentType(fileName);
        }

        public string GetRequestBody()
        {
            if (_context.Request.InputStream != null && string.IsNullOrEmpty(_requestBody))
            {
                _requestBody = new StreamReader(_context.Request.InputStream).ReadToEnd();
                return _requestBody;
            }
            else
            {
                return _requestBody;
            }
        }

        public T GetRequestBody<T>()
        {
            string body = GetRequestBody();
            try
            {
                if (!string.IsNullOrEmpty(body))
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error while converting data [{0}], Error :  {1}", body, ex.Message), ex);
                return default(T);
            }
        }

        public string GetQueryString(string key)
        {
            var qkey = _context.Request.QueryString.AllKeys.FirstOrDefault(f => f.ToLower() == key.ToLower());
            return _context.Request.QueryString[qkey];
        }

        public string GetFormData(string key)
        {
            return _context.Request.Form[key];
        }


        public string GetContentType(string path)
        {
            FileInfo fi = new FileInfo(path);
            return GetContentType(fi);
        }
        public string GetContentType(FileInfo pathInfo)
        {
            if (_serverPagesContentType.ContainsKey(pathInfo.Extension))
            {
                return _serverPagesContentType[pathInfo.Extension];
            }
            else
            {
                return MimeMapping.GetMimeMapping(pathInfo.FullName);
            }
        }


        public string SessionID
        {
            get
            {
                if (_context.Request.Cookies[CommonConst.CommonValue.SESSION_COOKIE] != null)
                {
                    return _context.Request.Cookies[CommonConst.CommonValue.SESSION_COOKIE].Value;
                }
                else
                {
                    return null;
                }

            }
        }
        public void ResetSession()
        {
            _context.Response.Cookies.Remove(CommonConst.CommonValue.SESSION_COOKIE);
        }
        public void UploadAppDomain()
        {
            System.Web.HttpRuntime.UnloadAppDomain();
        }

        public string GetHeader(string key)
        {
           return _context.Request.Headers[key];
        }

        public Dictionary<string, string> GetHeaders()
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            foreach (var key in _context.Request.Headers.AllKeys)
            {
                headers[key] = _context.Request.Headers[key];
            }
            return headers;
        }
    }
}