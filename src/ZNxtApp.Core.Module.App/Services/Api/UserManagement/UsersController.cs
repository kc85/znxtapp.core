using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.UserManagement
{
    public class UsersController : ViewBaseService
    {
        public UsersController(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject Get()
        {
            try
            {
                JArray joinData = new JArray();
                JObject collectionJoin = GetCollectionJoin(CommonConst.CommonField.USER_ID,CommonConst.Collection.USER_INFO, CommonConst.CommonField.USER_ID, null,"user_info");
                joinData.Add(collectionJoin);
                return GetPaggedData(CommonConst.Collection.USERS, joinData);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("UsersController.Get {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

        
    }
}
