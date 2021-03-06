﻿using System;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.CronJobs
{
    public class SessionCleanup : CronServiceBase
    {
        public SessionCleanup(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public int CleanSession()
        {
            double duration = ApplicationConfig.SessionDuration;
            double.TryParse(AppSettingService.GetAppSettingData(CommonConst.CommonField.SESSION_DURATION), out duration);

            var expirestime = CommonUtility.GetUnixTimestamp(DateTime.Now.AddMinutes(-duration));
            string filter = "{" + CommonConst.CommonField.UPDATED_DATE_TIME + " : { $lt : " + expirestime + "}}";
            DBProxy.Delete(CommonConst.Collection.SESSION_DATA, filter);

            return 1;
        }
    }
}