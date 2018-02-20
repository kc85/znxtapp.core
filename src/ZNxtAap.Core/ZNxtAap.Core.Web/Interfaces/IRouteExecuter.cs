using System;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
namespace ZNxtAap.Core.Web.Interfaces
{
    public interface IRouteExecuter
    {
        void Exec(RoutingModel route, IHttpContextProxy httpProxy);
    }
}
