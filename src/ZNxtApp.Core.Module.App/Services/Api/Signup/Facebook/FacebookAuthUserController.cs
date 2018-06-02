using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Module.App.Consts;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.Collections.Generic;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Enums;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class FacebookAuthUserController: ViewBaseService
    {
        private string AUTH_TOKEN = "auth_token";
        public FacebookAuthUserController(ParamContainer paramContainer) : base(paramContainer)
        {

        }

        public JObject Auth()
        {
            var request = HttpProxy.GetRequestBody<JObject>();
            if (request[AUTH_TOKEN] == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            var authToken = request[AUTH_TOKEN].ToString();
            var redirect_url = HttpProxy.GetQueryString(CommonConst.CommonField.REDIRECT_URL_KEY);


            var URL = string.Format("https://graph.facebook.com/v2.8/me?access_token={0}&method=get&pretty=0&sdk=joey&suppress_http_code=1", authToken);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync("").Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsStringAsync().Result;
                JObject responseJson = JObject.Parse(dataObjects);
                if (responseJson[CommonConst.CommonField.DISPLAY_ID] == null || responseJson[CommonConst.CommonField.NAME] == null)
                {
                    
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                else
                {

                    var userId = responseJson[CommonConst.CommonField.DISPLAY_ID].ToString();
                    if (CreateUser(userId, responseJson[CommonConst.CommonField.NAME].ToString()))
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
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS,null, resonseData);
                    }
                    else
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }

        }
        private bool CreateUser(string userid , string name)
        {
            JObject userData = new JObject();
            userData[CommonConst.CommonField.USER_ID] = userData[CommonConst.CommonField.DATA_KEY] = userid;
            userData[CommonConst.CommonField.USER_TYPE] = UserIDType.Facebook.ToString() ;


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
            return DBProxy.Write(CommonConst.Collection.USERS, userData,updateFilter,true);
        }

    }
}
