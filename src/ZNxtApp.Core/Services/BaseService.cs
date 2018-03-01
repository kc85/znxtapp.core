﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public abstract class BaseService
    {   
        protected IDBService DBProxy { get; private set; }
        protected IHttpContextProxy HttpProxy { get; private set; }
        protected ILogger Logger { get; private set; }
        protected IActionExecuter ActionExecuter { get; private set; }
        protected IPingService PingService { get; private set; }
        protected ResponseBuilder ResponseBuilder { get; private set; }
        protected IAppSettingService AppSettingService { get; private set; }

        public BaseService(ParamContainer paramContainer)
        {
            DBProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_DBPROXY);
            HttpProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
            Logger = paramContainer.GetKey(CommonConst.CommonValue.PARAM_LOGGER);
            ActionExecuter = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ACTIONEXECUTER);
            PingService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_PING_SERVICE);
            ResponseBuilder = paramContainer.GetKey(CommonConst.CommonValue.PARAM_RESPONBUILDER);
            AppSettingService = paramContainer.GetKey(CommonConst.CommonValue.PARAM_APP_SETTING);
        }
    }
}