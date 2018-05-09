using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Enums;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Helpers;

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
