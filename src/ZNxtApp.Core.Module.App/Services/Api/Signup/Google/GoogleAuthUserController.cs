using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class GoogleAuthUserController : SocialLoginBase
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
                if (isOauthVerification)
                {
                    return ValidateOAuthTokenCreateUser(authToken);
                }
                else
                {
                    if (request[CommonConst.CommonField.USER_ID] == null || request[CommonConst.CommonField.NAME] == null)
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                    }

                    Logger.Debug(string.Format("User id:{0}, Name:{1}, AuthToken:{2}", request[CommonConst.CommonField.USER_ID].ToString(), request[CommonConst.CommonField.NAME].ToString(), authToken));
                    if (CreateUser(request[CommonConst.CommonField.USER_ID].ToString(), request[CommonConst.CommonField.NAME].ToString(), UserIDType.Google))
                    {
                        return CreateSesssion(authToken, request[CommonConst.CommonField.USER_ID].ToString());
                    }
                    else
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                }


            }
            catch (Exception)
            {
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
        private JObject ValidateOAuthTokenCreateUser(string authToken)
        {
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
                    if (CreateUser(userId, responseJson[CommonConst.CommonField.NAME].ToString(), UserIDType.Google))
                    {
                        return CreateSesssion(authToken, userId, redirect_url);
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
                Logger.Debug("Http  error " + response.RequestMessage);
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }
        }
    }
}