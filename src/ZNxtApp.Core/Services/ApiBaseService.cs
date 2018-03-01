using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public class ApiBaseService : BaseService
    {
        protected RoutingModel Route { get; private set; }

        public ApiBaseService(ParamContainer paramContainer)
            : base(paramContainer)
        {
            Route = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ROUTE);
        }
    }
}
