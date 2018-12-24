using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Module.App.Services.Api.Signup;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.UserManagement
{
    public class UsersController : ViewBaseService
    {
        protected string OAUTH_VERIFICATION_ON_HOST = "oauth_verification_on_host";
        protected bool isOauthVerification = true;

        public UsersController(ParamContainer paramContainer) : base(paramContainer)
        {
            bool.TryParse(AppSettingService.GetAppSettingData(OAUTH_VERIFICATION_ON_HOST), out isOauthVerification);

        }

        public JObject Get()
        {
            try
            {
                JArray joinData = new JArray();
                JObject collectionJoin = GetCollectionJoin(CommonConst.CommonField.USER_ID, CommonConst.Collection.USER_INFO, CommonConst.CommonField.USER_ID, null, ModuleAppConsts.Field.USER_INFO);
                joinData.Add(collectionJoin);
                return GetPaggedData(CommonConst.Collection.USERS, joinData);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("UsersController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        public JObject GetSessionUser()
        {
            try
            {
                var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);

                if (user == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                else
                {
                    if (SignupHelper.IsValidToken(user, SessionProvider, Logger, isOauthVerification))
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(user)));
                    }
                    else
                    {
                        SessionProvider.ResetSession();
                        return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("UsersController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        public JObject GetUserInfo()
        {
            try
            {
                var user_id = HttpProxy.GetQueryString(CommonConst.CommonField.USER_ID);
                if (string.IsNullOrEmpty(user_id))
                {
                    Logger.Debug("User id query string is null");
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                var userData = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (userData == null)
                {
                    Logger.Debug("User session data is null");
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                if (userData.user_id != user_id)
                {
                    Logger.Debug("User id conflict with session data");
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                if (SignupHelper.IsValidToken(userData, SessionProvider, Logger, isOauthVerification))
                {
                    JArray joinData = new JArray();
                    JObject collectionJoin = GetCollectionJoin(CommonConst.CommonField.USER_ID, CommonConst.Collection.USER_INFO, CommonConst.CommonField.USER_ID, null, ModuleAppConsts.Field.USER_INFO);
                    joinData.Add(collectionJoin);
                    JObject filter = new JObject();
                    filter[CommonConst.CommonField.USER_ID] = user_id;
                    var data = GetPaggedData(CommonConst.Collection.USERS, joinData, filter.ToString());
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data[CommonConst.CommonField.DATA][0]);
                }
                else
                {
                    Logger.Debug("User Auth token is not valid");
                    SessionProvider.ResetSession();
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("UsersController.GetUserInfo {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}