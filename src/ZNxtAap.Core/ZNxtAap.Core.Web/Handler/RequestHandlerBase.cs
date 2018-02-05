﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.DB.Mongo;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Web.Proxies;
using ZNxtAap.Core.Web.Services;
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
        protected HttpContext _httpContext;
        // protected IExecuteController _executeController;
        protected IHttpContextProxy _httpProxy;
        protected IRoutings _routings;
        protected IAppInstaller _appInstaller;
        protected IPingService _pingService;
        //protected IStaticContentHandler _staticContentHandler;
        // protected ILogger _logger;
        public RequestHandlerBase()
        {
            _pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));
            _appInstaller = Installer.GetInstance(_pingService, new Helpers.DataBuilderHelper());
        }

        public virtual void ProcessRequest(HttpContext context)
        {
            _httpContext = context;
            _httpProxy = new HttpContextProxy(context);
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
    }   
}
