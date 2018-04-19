using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.CronJobs
{
    public class OTPCleanUpCronJob : CronServiceBase
    {
        public OTPCleanUpCronJob(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public int UpdateOTPStatus()
        {
            return 1;
        }
    }
}
