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

        protected HttpContext _httpContext;

        // protected IExecuteController _executeController;
        protected IHttpContextProxy _httpProxy;

        protected IRoutings _routings;
        protected IAppInstaller _appInstaller;
        protected IPingService _pingService;
        protected IDBService _dbProxy;
        //protected IStaticContentHandler _staticContentHandler;
        protected IRouteExecuter _routeExecuter;
        protected ILogger _logger;
        public RequestHandlerBase()
        {
            InitApp.Run();
            _logger = Logger.GetLogger(this.GetType().Name);
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
            _pingService = new PingService(new MongoDBService(ApplicationConfig.DataBaseName, CommonConst.Collection.PING));

            var appInstallerLogger = Logger.GetLogger(typeof(Installer).Name);
            _appInstaller = Installer.GetInstance(
                _pingService,
                new Helpers.DataBuilderHelper(),
                appInstallerLogger,
                _dbProxy,
                new EncryptionService(),
                new ModuleInstaller.Installer.Installer(appInstallerLogger, _dbProxy));
            _routings = Routings.Routings.GetRoutings(_dbProxy, _logger);
            _routeExecuter = new RouteExecuter();
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