using System.Web;
using ZNxtAap.Core.Config;

namespace ZNxtAap.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {
        public override void ProcessRequest(HttpContext context)
        {
            base.ProcessRequest(context);

            //var requestUriPath = _httpProxy.GetURIAbsolutePath();

            //  var route = _routings.GetRoute(_httpProxy.GetHttpMethod(), requestUriPath);
            if (ApplicationMode.Maintance == ApplicationConfig.GetApplicationMode && _appInstaller.Status != Enums.AppInstallStatus.Finish)
            {
                _appInstaller.Install(_httpProxy);
            }
            //else if (route != null)
            //{
            //    // TODO handle the routes

            //}

            //else
            //{
            //    // Handle the  non route request

            //}
            WriteResponse();
        }
    }
}