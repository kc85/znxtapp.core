using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.CronJobs
{
    public class CheckAppHealthCronJob : CronServiceBase
    {
        public CheckAppHealthCronJob(ParamContainer pamamContainer)
            : base(pamamContainer)
        {

        }
        public int Check()
        {
            return 1;
        }
    }
}
