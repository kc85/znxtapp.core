using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Interfaces
{
    public interface IRoutings
    {
        RoutingModel GetRoute(string Method, string url);

        void LoadRoutes();
    }
}