using ZNxtAap.Core.Model;

namespace ZNxtAap.Core.Interfaces
{
    public interface IRoutings
    {
        RoutingModel GetRoute(string Method, string url);
    }
}