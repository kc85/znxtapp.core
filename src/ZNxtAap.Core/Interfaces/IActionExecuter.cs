using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Model;

namespace ZNxtAap.Core.Interfaces
{
    public interface IActionExecuter
    {
        object Exec(RoutingModel route, ParamContainer helper);
    }
}
