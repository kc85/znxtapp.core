using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public class FacebookAuthUserController : SocialLoginBase
    {
      
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
                if (CreateUser(request[CommonConst.CommonField.USER_ID].ToString(), request[CommonConst.CommonField.NAME].ToString(),UserIDType.Facebook))
                {
                    return CreateSesssion(authToken, request[CommonConst.CommonField.USER_ID].ToString());
                }
                else
                {
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
            }
            
        }

        private JObject ValidateOAuthTokenCreateUser(string authToken)
        {
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
                    if (CreateUser(userId, responseJson[CommonConst.CommonField.NAME].ToString(), UserIDType.Facebook))
                    {
                        return CreateSesssion(authToken,userId, redirect_url);
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

       

       
    }
}