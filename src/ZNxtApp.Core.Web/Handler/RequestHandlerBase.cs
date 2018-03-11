using System;
using System.Net;
using System.Web;
using System.Web.SessionState;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.AppStart;
using ZNxtApp.Core.Web.Interfaces;
using ZNxtApp.Core.Web.Proxies;
using ZNxtApp.Core.Web.Services;
using ZNxtApp.Core.Web.Util;
using ZNxtApp.Core.AppInstaller;
using System.IO;
using ZNxtApp.Core.Web.Helper;
using System.Collections.Generic;

namespace ZNxtApp.Core.Web.Handler
{
    public abstract class RequestHandlerBase : IHttpHandler
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private HttpContext _httpContext;
        protected IHttpContextProxy _httpProxy;
        protected IInitData _initData;
        protected IRoutings _routings;
        protected IRouteExecuter _routeExecuter;
        protected ILogger _logger;
        protected IViewEngine _viewEngine;
        public RequestHandlerBase()
        {
            _viewEngine = ViewEngine.GetEngine();
            _routeExecuter = new RouteExecuter();
        }

        private void CreateRoute()
        {
            _routings = Routings.Routings.GetRoutings();
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            HandleSession(context);
            _httpProxy = new HttpContextProxy(context);
            _initData = _httpProxy;
            _logger = Logger.GetLogger(this.GetType().Name, _httpProxy.TransactionId);
            CreateRoute();
            _httpContext = context;
        }

        protected void WriteResponse()
        {
            _httpContext.Response.StatusCode = _httpProxy.ResponseStatusCode;
            _httpContext.Response.StatusDescription = _httpProxy.ResponseStatusMessage;
            _httpContext.Response.ContentType = _httpProxy.ContentType;

            if (_httpProxy.Response != null)
            {
                _httpContext.Response.OutputStream.Write(_httpProxy.Response, 0, _httpProxy.Response.Length);
            }
            RemoveHeaders();
        }
        private void RemoveHeaders()
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-SourceFiles");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
            HttpContext.Current.Response.Headers.Remove("X-AspNetMvc-Version");
            HttpContext.Current.Response.Headers.Remove("X-AspNet-Version");
        }

        protected void HandleStaticContent(string requestUriPath)
        {
            var dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            var fi = new FileInfo(requestUriPath);
            if (fi.Extension == CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_EXTENSION)
            {
               
                var data = StaticContentHandler.GetStringContent(dbProxy, _logger, requestUriPath);
                var pageModel = new Dictionary<string, dynamic>();
                data = _viewEngine.Compile(data, requestUriPath, ServerPageModelHelper.SetDefaultModel(dbProxy, _httpProxy, _logger, _viewEngine, pageModel, fi.DirectoryName));
                if (pageModel.ContainsKey(CommonConst.CommonValue.PAGE_TEMPLATE_PATH))
                {
                    var templateFileData = StaticContentHandler.GetStringContent(dbProxy, _logger, pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH]);
                    pageModel[CommonConst.CommonValue.RENDERBODY_DATA] = data;
                    data = _viewEngine.Compile(templateFileData, pageModel[CommonConst.CommonValue.PAGE_TEMPLATE_PATH], ServerPageModelHelper.SetDefaultModel(dbProxy, _httpProxy, _logger, _viewEngine, pageModel));
                }

                if (data != null)
                {
                    _httpProxy.SetResponse(CommonConst._200_OK, data);
                }
                else
                {
                    _httpProxy.SetResponse(CommonConst._404_RESOURCE_NOT_FOUND);
                }
                _httpProxy.ContentType = CommonConst.CONTENT_TYPE_TEXT_HTML;
            }
            else
            {
                var data = StaticContentHandler.GetContent(dbProxy, _logger, requestUriPath);
                if (data != null)
                {
                    _httpProxy.SetResponse(CommonConst._200_OK, data);
                }
                else
                {
                    _httpProxy.SetResponse(CommonConst._404_RESOURCE_NOT_FOUND, data);
                }
                _httpProxy.ContentType = MimeMapping.GetMimeMapping(requestUriPath);
            }
            
            if (ApplicationConfig.StaticContentCache)
            {
                _httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
                _httpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(10));
                _httpContext.Response.Cache.SetMaxAge(new TimeSpan(10, 0, 0, 0));
            }

            if (ApplicationMode.Maintance == ApplicationConfig.GetApplicationMode)
            {
                _httpContext.Response.Headers[string.Format("{0}.{1}", CommonConst.CommonField.HTTP_RESPONE_DEBUG_INFO, CommonConst.CommonValue.TIME_SPAN)] = (DateTime.Now - _initData.InitDateTime).TotalMilliseconds.ToString();
                _httpContext.Response.Headers[string.Format("{0}.{1}", CommonConst.CommonField.HTTP_RESPONE_DEBUG_INFO, CommonConst.CommonField.TRANSATTION_ID)] = _initData.TransactionId;
            }
            RemoveHeaders();
        }

        private void HandleSession(HttpContext context)
        {
            if(context.Request.Cookies[CommonConst.CommonValue.SESSION_COOKIE]==null)
            {
                CreateSession(context);
            }
        }

        private void CreateSession(HttpContext context)
        {
            var cookie = new HttpCookie(CommonConst.CommonValue.SESSION_COOKIE,Guid.NewGuid().ToString());
            var expires = DateTime.Now.AddMinutes(ApplicationConfig.SessionDuration);            
            cookie.Expires = expires;
            cookie.HttpOnly = true;
            context.Response.Cookies.Add(cookie);
        }
    }
}