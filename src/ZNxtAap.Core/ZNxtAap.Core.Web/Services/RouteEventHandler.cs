using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class RouteEventHandler
    {
        public bool ExecBeforeEvent(IActionExecuter actionExecuter, RoutingModel route, ParamContainer paramContainer)
        {
            IDBService dbProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);
            ILogger logger = paramContainer.GetKey(CommonConst.CommonValue.PARAM_LOGGER);
            foreach(var eventSubscriber in EventSubscription.GetInstance(dbProxy,logger).GetEvent(route.GetEventName()))
            {

            }
            return true;

        }
        public bool ExecAfterEvent(IActionExecuter actionExecuter, RoutingModel route, ParamContainer pamamContainer, object objResult)
        {
            return true;
        }
    }
}
