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

namespace ZNxtApp.Core.Module.App.Services.Api.Menu
{
    public class BackendMenuController : ApiBaseService
    {
        public BackendMenuController(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject GetMenuItems()
        {
            try
            {
                Logger.Debug("Enter to GetMenuItems");
                DBProxy.Collection = ModuleAppConsts.Collection.BACKEND_UI_ROUTES;


                // TODO need to do the session user filter;
                var data = DBProxy.Get(CommonConst.Filters.IS_OVERRIDE_FILTER);
                List<string> userAccess = new List<string>() { "*" };
                var sessionUser = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (sessionUser != null)
                {
                    userAccess.AddRange(sessionUser.groups);
                }
                JArray menuData = new JArray();
                foreach (var item in data)
                {
                    if (item["auth_users"].Where(f => userAccess.Contains(f.ToString())).Any())
                    {
                        menuData.Add(item);
                    }
                }
                Logger.Debug("Got GetMenuItems");
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, menuData);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error in  GetMenuItems: {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }
    }
}
