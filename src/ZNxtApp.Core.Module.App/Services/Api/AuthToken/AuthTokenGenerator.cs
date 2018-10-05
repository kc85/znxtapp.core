using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.AuthToken
{
    public class AuthTokenGenerator : ApiBaseService
    {
        private const int MAX_KEYS = 5;

        public AuthTokenGenerator(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public JObject Generate()
        {
            try
            {
                var userData = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (userData == null)
                {
                    Logger.Debug("User session data is null");
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }

                DBQuery query = new DBQuery() { Filters = new FilterQuery() { new Filter(CommonConst.CommonField.USER_ID, userData.user_id) } };
                if (DBProxy.GetCount(ModuleAppConsts.Collection.AUTH_TOKEN_COLLECTION, query.Filters) >= MAX_KEYS)
                {
                    return ResponseBuilder.CreateReponse(ApiKeyResponseCode._MAX_AUTH_TOKEN_REACHED);
                }

                var apikey = GenerateApiKey();
                var data = GenerateApiKeyData(userData, apikey);

                if (DBProxy.WriteData(ModuleAppConsts.Collection.AUTH_TOKEN_COLLECTION, data, false))
                {
                    data[ModuleAppConsts.Field.AUTH_TOKEN] = apikey;
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS,data);
                }
                else
                {
                    Logger.Error("Error in writing data");
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        private string GenerateApiKey()
        {
           return string.Format("{0}{1}{2}{3}", CommonUtility.RandomString(10), CommonUtility.RandomNumber(6), CommonUtility.RandomString(10), CommonUtility.RandomNumber(6));
        }
        private JObject GenerateApiKeyData(UserModel userData,string key)
        {
            JObject data = new JObject();

            data[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            data[CommonConst.CommonField.USER_ID] = userData.user_id;
            data[ModuleAppConsts.Field.AUTH_TOKEN] = EncryptionService.Encrypt(key);
            data[CommonConst.CommonField.IS_ENABLED] = true;

            return data;
        }
    }
}