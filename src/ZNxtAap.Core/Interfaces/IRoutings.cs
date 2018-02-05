using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Module;

namespace ZNxtAap.Core.Interfaces
{
    public interface IRoutings
    {
        RoutingModel GetRoute(string Method, string url);

    }
}
