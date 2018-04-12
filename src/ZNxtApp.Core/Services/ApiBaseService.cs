using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public abstract class ApiBaseService : BaseService
    {
        protected RoutingModel Route { get; private set; }
        protected ISessionProvider SessionProvider { get; private set; }
        protected IHttpContextProxy HttpProxy { get; private set; }

        public ApiBaseService(ParamContainer paramContainer)
            : base(paramContainer)
        {
            HttpProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
            Route = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ROUTE);
            SessionProvider = paramContainer.GetKey(CommonConst.CommonValue.PARAM_SESSION_PROVIDER);
        }
    }
}
