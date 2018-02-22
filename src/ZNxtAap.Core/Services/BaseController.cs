using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;

namespace ZNxtAap.Core.Services
{
    public abstract class BaseController
    {
        protected RoutingModel Route { get; private set; }
        protected IDBService DBProxy { get; private set; }
        protected IHttpContextProxy HttpProxy { get; private set; }
        protected ILogger Logger { get; private set; }
        protected IActionExecuter ActionExecuter { get; private set; }
        protected IPingService PingService { get; private set; }
        protected ResponseBuilder ResponseBuilder { get; private set; }
        protected IAppSettingService AppSettingService { get; private set; }
        public BaseController(ParamContainer pamamContainer)
        {
            Route = pamamContainer[CommonConst.CommonValue.PARAM_ROUTE]();
            DBProxy = pamamContainer[CommonConst.CommonValue.PARAM_DBPROXY]();
            HttpProxy = pamamContainer[CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY]();
            Logger = pamamContainer[CommonConst.CommonValue.PARAM_LOGGER]();
            ActionExecuter = pamamContainer[CommonConst.CommonValue.PARAM_ACTIONEXECUTER]();
            PingService = pamamContainer[CommonConst.CommonValue.PARAM_PING_SERVICE]();
            ResponseBuilder = pamamContainer[CommonConst.CommonValue.PARAM_RESPONBUILDER]();
            AppSettingService = pamamContainer[CommonConst.CommonValue.PARAM_APP_SETTING]();
        }
    }
}
