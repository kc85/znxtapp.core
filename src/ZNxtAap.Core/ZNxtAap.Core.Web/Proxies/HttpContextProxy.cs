using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Web.Services;

namespace ZNxtAap.Core.Web.Proxies
{
    public class HttpContextProxy : IHttpContextProxy
    {
        private HttpContext _context;
        private ILogger _logger;
        private DataBuilderHelper _dataBuilderHelper;
        private byte[] _response;

        public byte[] Response
        {
            get { return _response; }
        }

        private int _responseStatusCode;
        private string _responseStatusMessage;
        public int ResponseStatusCode { get { return _responseStatusCode; } }
        public string ResponseStatusMessage { get { return _responseStatusMessage; } }
        public string ContentType { get; set; }

        public HttpContextProxy(HttpContext context)
        {
            _context = context;
            _logger = Logger.GetLogger(this.GetType().FullName);
            _dataBuilderHelper = new DataBuilderHelper();
            _responseStatusCode = (int)HttpStatusCode.OK;
            _responseStatusMessage = HttpStatusCode.OK.ToString();
            ContentType = CommonConst.CONTENT_TYPE_TEXT_HTML;
        }

        public string GetURIAbsolutePath()
        {
            var path = _context.Request.Url.AbsolutePath.ToLower();
            if (path.IndexOf(ApplicationConfig.AppPath.ToLower()) == 0)
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
            return MimeMapping.GetMimeMapping(fileName);
        }

        public string GetRequestBody()
        {
            if (_context.Request.InputStream != null)
            {
                return new StreamReader(_context.Request.InputStream).ReadToEnd();
            }
            else
            {
                return string.Empty;
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
    }
}