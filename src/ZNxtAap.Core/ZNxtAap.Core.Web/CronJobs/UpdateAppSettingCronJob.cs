using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Services;
using ZNxtAap.Core.Web.Services;

namespace ZNxtAap.Core.Web.CronJobs
{
    public class UpdateAppSettingCronJob :  CronServiceBase
    {
        public UpdateAppSettingCronJob(ParamContainer pamamContainer)
            : base(pamamContainer)
        {

        }
        public int Update()
        {
            (AppSettingService as AppSettingService).ReloadSettings(true);
            return 1;
        }
    }
}
