using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.Signup
{
    public abstract class UserRegistrationBase : ViewBaseService

    {
        protected const string USER_REGISTRATION_CAPCHA_VALIDATION_SESSION_KEY = "registration_capcha_check";

        public UserRegistrationBase(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        protected UserModel GetUserDataFromRequest(JObject request)
        {
            UserModel user = new UserModel();
            user.user_id = request[CommonConst.CommonField.USER_ID].ToString();
            var userIdTypeStr = request[CommonConst.CommonField.USER_TYPE].ToString();

            UserIDType userIdType = UserIDType.PhoneNumber;
            Enum.TryParse<UserIDType>(userIdTypeStr, out userIdType);
            user.user_type = userIdType.ToString();
            return user;
        }

        protected bool IsUserExists(string userId, UserIDType userIdType = UserIDType.PhoneNumber)
        {
            return GetUser(userId, userIdType) != null;
        }

        protected JObject GetUser(string userId, UserIDType userIdType)
        {
            return DBProxy.FirstOrDefault<JObject>(CommonConst.Collection.USERS, CommonConst.CommonField.USER_ID, userId);
        }
    }
}