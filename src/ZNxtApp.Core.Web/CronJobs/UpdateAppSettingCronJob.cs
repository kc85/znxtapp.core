using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.CronJobs
{
    public class UpdateAppSettingCronJob : CronServiceBase
    {
        public UpdateAppSettingCronJob(ParamContainer pamamContainer)
            : base(pamamContainer)
        {
        }

        public int Update()
        {
            Logger.Debug("Update Setting Cache from UpdateAppSettingCronJob.Update");
            (AppSettingService as AppSettingService).ReloadSettings(true);
            EventSubscription.GetInstance(DBProxy, Logger).LoadSubscriptions(true);
            return 1;
        }
    }
}