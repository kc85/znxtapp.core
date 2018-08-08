using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class GoogleAuthUserController : ViewBaseService
    {
        
     
        public GoogleAuthUserController(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject Auth()
        {
            try
            {


                var request = HttpProxy.GetRequestBody<JObject>();
                if (request[ModuleAppConsts.Field.AUTH_TOKEN] == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                var authToken = request[ModuleAppConsts.Field.AUTH_TOKEN].ToString();
                var redirect_url = HttpProxy.GetQueryString(CommonConst.CommonField.REDIRECT_URL_KEY);

                var URL = string.Format("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token={0}", authToken);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                HttpResponseMessage response = client.GetAsync("").Result;
                Logger.Debug("Getting data");
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(dataObjects);
                    Logger.Debug("Data", responseJson);
                    if (responseJson["sub"] != null)
                    {
                        var userId = responseJson["sub"].ToString();
                        if (SignupHelper.CreateUser(DBProxy, AppSettingService, userId, Enums.UserIDType.Google, responseJson["name"].ToString(), responseJson["email"].ToString(), responseJson["picture"].ToString()))
                        {
                            var user = DBProxy.FirstOrDefault<UserModel>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, userId);
                            SessionProvider.SetValue(CommonConst.CommonValue.SESSION_USER_KEY, user);
                            SessionProvider.SetValue(ModuleAppConsts.Field.AUTH_TOKEN, authToken);

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
                            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, resonseData);
                        }
                        else
                        {
                            return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                        }
                    }
                    else
                    {
                       
                        return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                    }

                }
                else
                {
                    Logger.Debug("Http  error "+ response.RequestMessage);
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
            }
            catch (Exception)
            {
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

    }
}
