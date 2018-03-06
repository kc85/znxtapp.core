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
        protected IRoutings _routings;
        protected IRouteExecuter _routeExecuter;
        protected ILogger _logger;
        public RequestHandlerBase()
        {
          
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
        }

        protected void HandleStaticContent(string requestUriPath)
        {
            var dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            var data = StaticContentHandler.GetContent(dbProxy, _logger, requestUriPath);
            _httpProxy.ContentType = MimeMapping.GetMimeMapping(requestUriPath);
            if (ApplicationConfig.StaticContentCache)
            {
                _httpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
                _httpContext.Response.Cache.SetExpires(DateTime.Now.AddDays(10));
                _httpContext.Response.Cache.SetMaxAge(new TimeSpan(10, 0, 0, 0));
            }
            if (data != null)
            {
                _httpProxy.SetResponse(CommonConst._200_OK, data);
            }
            else
            {
                _httpProxy.SetResponse(CommonConst._404_RESOURCE_NOT_FOUND, data);
            }
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