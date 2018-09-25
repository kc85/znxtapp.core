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