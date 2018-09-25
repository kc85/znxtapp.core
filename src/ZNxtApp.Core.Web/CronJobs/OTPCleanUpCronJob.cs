using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
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
            int duration = 15;
            int.TryParse(AppSettingService.GetAppSettingData(CommonConst.CommonField.OTP_DURATION), out duration);

            var expirestime = CommonUtility.GetUnixTimestamp(DateTime.Now.AddMinutes(-duration));
            string filter = "{" + CommonConst.CommonField.CREATED_DATA_DATE_TIME + " : { $lt : " + expirestime + "}}";

            DBProxy.Delete(CommonConst.Collection.OTPs, filter);
            return 1;
        }
    }
}