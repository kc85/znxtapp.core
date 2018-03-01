using System;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
namespace ZNxtApp.Core.Web.Interfaces
{
    public interface IRouteExecuter
    {
        void Exec(RoutingModel route, IHttpContextProxy httpProxy);
    }
}
