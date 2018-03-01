using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class RouteEventHandler
    {
        public bool ExecBeforeEvent(IActionExecuter actionExecuter, RoutingModel route, ParamContainer paramContainer)
        {
            ExecEvent(actionExecuter, route, paramContainer, Models.ExecutionEventType.Init);
            return true;

        }

        private static void ExecEvent(IActionExecuter actionExecuter, RoutingModel route, ParamContainer paramContainer,Models.ExecutionEventType eventType)
        {
            IDBService dbProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);
            ILogger logger = paramContainer.GetKey(CommonConst.CommonValue.PARAM_LOGGER);
            foreach (var eventSubscriber in EventSubscription.GetInstance(dbProxy, logger).GetSubscriptions(route.GetEventName(), eventType))
            {
                actionExecuter.Exec(eventSubscriber.ExecultAssembly, eventSubscriber.ExecuteType, eventSubscriber.ExecuteMethod, paramContainer);
            }
        }
        public bool ExecAfterEvent(IActionExecuter actionExecuter, RoutingModel route, ParamContainer paramContainer, object objResult)
        {
            ExecEvent(actionExecuter, route, paramContainer, Models.ExecutionEventType.Completed);              
            return true;
        }
    }
}
