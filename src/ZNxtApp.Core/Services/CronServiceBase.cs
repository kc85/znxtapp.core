using System;
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
    public abstract class CronServiceBase : BaseService
    {   
        protected IRoutings Routings { get; private set; }

        public CronServiceBase(ParamContainer paramContainer)
            : base(paramContainer)
        {   
            Routings = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ROUTING_OBJECT);
        }
    }
}
