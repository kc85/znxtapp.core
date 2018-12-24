using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public  abstract class SocialLoginBase : ViewBaseService
    {
        protected string AUTH_TOKEN = "auth_token";
        protected string OAUTH_VERIFICATION_ON_HOST = "oauth_verification_on_host";
        protected bool isOauthVerification = true;
        public SocialLoginBase(ParamContainer paramContainer) : base(paramContainer)
        {
            bool.TryParse(AppSettingService.GetAppSettingData(OAUTH_VERIFICATION_ON_HOST), out isOauthVerification);
        }
        protected bool CreateUser(string userid, string name, UserIDType userType)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = userid;
            userData[CommonConst.CommonField.USER_TYPE] = userType.ToString();

            var updateFilter = userData.ToString();

            userData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            userData[CommonConst.CommonField.IS_ENABLED] = true;
            userData[CommonConst.CommonField.NAME] = name;
            userData[CommonConst.CommonField.IS_EMAIL_VALIDATE] = false;
            userData[CommonConst.CommonField.IS_PHONE_VALIDATE] = false;
            var userGroup = AppSettingService.GetAppSetting(ModuleAppConsts.Field.DEFAULT_USER_GROUPS_APP_SETTING_KEY);
            Logger.Debug("userGroup setting data", userGroup);
            if (userGroup == null)
            {
                throw new Exception("User Group setting not found in AppSettings");
            }
            if (userGroup[CommonConst.CommonField.DATA] == null || userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] == null)
            {
                throw new Exception("User Groups not found in AppSettings");
            }
            userData[CommonConst.CommonField.USER_GROUPS] = (userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] as JArray);
            return DBProxy.Write(CommonConst.Collection.USERS, userData, updateFilter, true);
        }
        protected JObject CreateSesssion(string authToken, string userId, string redirect_url = null)
        {
            var user = DBProxy.FirstOrDefault<UserModel>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, userId);
            SessionProvider.SetValue(CommonConst.CommonValue.SESSION_USER_KEY, user);
            SessionProvider.SetValue(AUTH_TOKEN, authToken);

            var rurl = AppSettingService.GetAppSettingData(ModuleAppConsts.Field.SIGNUP_LENDING_PAGE_SETTING_KEY);

            JObject resonseData = new JObject();
            if (string.IsNullOrEmpty(redirect_url))
            {
                resonseData[CommonConst.CommonField.REDIRECT_URL_KEY] = rurl;
            }
            else
            {
                resonseData[CommonConst.CommonField.REDIRECT_URL_KEY] = string.Format("{0}?{1}={2}", rurl, CommonConst.CommonField.REDIRECT_URL_KEY, redirect_url);
            }
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, null, resonseData);
        }
    }
}
