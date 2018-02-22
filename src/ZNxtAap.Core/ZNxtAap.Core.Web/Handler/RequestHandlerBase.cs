using System;
using System.Net;
using System.Web;
using System.Web.SessionState;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Web.AppStart;
using ZNxtAap.Core.Web.Interfaces;
using ZNxtAap.Core.Web.Proxies;
using ZNxtAap.Core.Web.Services;
using ZNxtAap.Core.Web.Util;
using ZNxtAap.CoreAppInstaller;

namespace ZNxtAap.Core.Web.Handler
{
    public abstract class RequestHandlerBase : IHttpHandler, IRequiresSessionState
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
            InitApp.Run();
            _routeExecuter = new RouteExecuter();
            
        }

        private void CreateRoute()
        {
            _routings = Routings.Routings.GetRoutings();
        }

        public virtual void ProcessRequest(HttpContext context)
        {
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
       
    }
}