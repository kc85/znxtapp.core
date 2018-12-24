using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public static class SignupHelper
    {

        public static bool CreateUser(IDBService dbProxy, IAppSettingService appSettingService, string userid, UserIDType userType, string name, string email, string picture)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = userid;
            userData[CommonConst.CommonField.USER_TYPE] = userType.ToString();

            var updateFilter = userData.ToString();

            userData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            userData[CommonConst.CommonField.IS_ENABLED] = true;
            userData[CommonConst.CommonField.NAME] = name;
            userData[CommonConst.CommonField.EMAIL] = email;

            userData[CommonConst.CommonField.IS_EMAIL_VALIDATE] = userType == UserIDType.Google ? true : false;
            userData[CommonConst.CommonField.IS_PHONE_VALIDATE] = false;
            var userGroup = appSettingService.GetAppSetting(ModuleAppConsts.Field.DEFAULT_USER_GROUPS_APP_SETTING_KEY);
            if (userGroup == null)
            {
                throw new Exception("User Group setting not found in AppSettings");
            }
            if (userGroup[CommonConst.CommonField.DATA] == null || userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] == null)
            {
                throw new Exception("User Groups not found in AppSettings");
            }
            userData[CommonConst.CommonField.USER_GROUPS] = (userGroup[CommonConst.CommonField.DATA][CommonConst.CommonField.USER_GROUPS] as JArray);
            if (dbProxy.Write(CommonConst.Collection.USERS, userData, updateFilter, true))
            {
                var userInfo = new JObject();
                userInfo[CommonConst.CommonField.USER_ID] = userid;
                var filter = userInfo.ToString();
                userInfo[CommonConst.CommonField.USER_PIC] = picture;

                var dbresponse = dbProxy.Update(CommonConst.Collection.USER_INFO, filter, userInfo, true, MergeArrayHandling.Merge);
                return dbresponse == 1;
            }
            else
            {
                return false;
            }
        }

        public static bool IsValidToken(UserModel user, ISessionProvider sessionProvider, ILogger logger, bool isOauthVerification =true)
        {
            if (!isOauthVerification) return true;

                if (user.user_type == UserIDType.Google.ToString())
            {
                var authToken = sessionProvider.GetValue<string>(ModuleAppConsts.Field.AUTH_TOKEN);
                if (string.IsNullOrEmpty(authToken))
                {
                    return false;
                }

                var URL = string.Format("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}", authToken);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(dataObjects);
                    logger.Debug("Data", responseJson);
                    if (responseJson["sub"] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}