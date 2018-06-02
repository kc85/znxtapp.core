using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Module.App.Consts;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Helpers;

namespace ZNxtApp.Core.Module.App.Services.Api.UserManagement
{
    public class UpdateUserInfo : ViewBaseService
    {
        public UpdateUserInfo(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject AdminUpdate()
        {
            try
            {
                Logger.Debug("Enter to UpdateUserInfo.AdminUpdate");
                JObject request = HttpProxy.GetRequestBody<JObject>();
                if (request == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                string userId = string.Empty;
                if (request[CommonConst.CommonField.USER_ID] != null)
                {
                    userId = request[CommonConst.CommonField.USER_ID].ToString();
                }
                if (string.IsNullOrEmpty(userId))
                {
                    Logger.Error(string.Format("Error in  UpdateUserInfo.AdminUpdate: {0}", "User id empty"));
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }

                JObject updateFilter = new JObject();
                updateFilter[CommonConst.CommonField.USER_ID] = userId;

                if (request[ModuleAppConsts.Field.USER_INFO] != null && (request[ModuleAppConsts.Field.USER_INFO] as JArray).Count > 0)
                {

                    var userInfo = request[ModuleAppConsts.Field.USER_INFO][0] as JObject;
                    Logger.Debug("Updating User Info", userInfo);
                    var dbresponse = DBProxy.Update(CommonConst.Collection.USER_INFO, updateFilter.ToString(), userInfo, true, MergeArrayHandling.Replace);
                    if (dbresponse == 0)
                    {
                        Logger.Error(string.Format("Error in  UpdateUserInfo.AdminUpdate: {0}, collection {1}", "Error in updating data in db", CommonConst.Collection.USER_INFO));
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                }

                JObject userdata = new JObject();
                userdata[CommonConst.CommonField.PHONE] = request[CommonConst.CommonField.PHONE];
                userdata[CommonConst.CommonField.EMAIL] = request[CommonConst.CommonField.EMAIL];
                userdata[CommonConst.CommonField.NAME] = request[CommonConst.CommonField.NAME];

                Logger.Debug("Updating User", userdata);
                if (DBProxy.Write(CommonConst.Collection.USERS, userdata, updateFilter.ToString(), false, MergeArrayHandling.Union))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, updateFilter);
                }
                else
                {
                    Logger.Error(string.Format("Error in  UpdateUserInfo.AdminUpdate: {0}, collection {1}", "Error in updating data in db", CommonConst.Collection.USERS));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error in  UpdateUserInfo.AdminUpdate: {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}
