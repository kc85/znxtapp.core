using ZNxtAap.Core.Module;

namespace ZNxtAap.Core.Interfaces
{
    public interface IRoutings
    {
        RoutingModel GetRoute(string Method, string url);
    }
}