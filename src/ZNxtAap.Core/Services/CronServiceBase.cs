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
    public abstract class CronServiceBase
    {   
        protected IDBService DBProxy { get; private set; }        
        protected ILogger Logger { get; private set; }
        protected IActionExecuter ActionExecuter { get; private set; }        
        protected ResponseBuilder ResponseBuilder { get; private set; }
        protected IAppSettingService AppSettingService { get; private set; }
        protected IPingService PingService { get; private set; }
        protected IRoutings Routings { get; private set; }
        public CronServiceBase(ParamContainer pamamContainer)
        {
            DBProxy = pamamContainer[CommonConst.CommonValue.PARAM_DBPROXY]();
            Logger = pamamContainer[CommonConst.CommonValue.PARAM_LOGGER]();
            ActionExecuter = pamamContainer[CommonConst.CommonValue.PARAM_ACTIONEXECUTER]();
            ResponseBuilder = pamamContainer[CommonConst.CommonValue.PARAM_RESPONBUILDER]();
            AppSettingService = pamamContainer[CommonConst.CommonValue.PARAM_APP_SETTING]();
            PingService = pamamContainer[CommonConst.CommonValue.PARAM_PING_SERVICE]();
            Routings = pamamContainer[CommonConst.CommonValue.PARAM_ROUTING_OBJECT]();
        }
    }
}
