using System;
using System.Net;
using System.Web;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Web.Util;

namespace ZNxtAap.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {
        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);

            var requestUriPath = _httpProxy.GetURIAbsolutePath();

            //  var route = _routings.GetRoute(_httpProxy.GetHttpMethod(), requestUriPath);
            if (ApplicationMode.Maintance == ApplicationConfig.GetApplicationMode && _appInstaller.Status != Enums.AppInstallStatus.Finish)
            {
                _appInstaller.Install(_httpProxy);
            }
            else
            {

                HandleStaticContent(requestUriPath);
            }
            WriteResponse();
        }

        private void HandleStaticContent(string requestUriPath)
        {
            var data = StaticContentHandler.GetContent(_dbProxy, _logger, requestUriPath);
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