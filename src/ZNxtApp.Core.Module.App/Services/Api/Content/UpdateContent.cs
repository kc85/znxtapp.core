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

namespace ZNxtApp.Core.Module.App.Services.Api.Content
{
    public class UpdateContent: ViewBaseService
    {
        private string _contentUpdateModuleName = "ContentUploader";
        private string _contentHistory= "history";

        public UpdateContent(ParamContainer paramContainer) : base(paramContainer)
        {
        }
        public JObject Update()
        {
            try
            {
                Logger.Debug(string.Format("Enter to UpdateContent.Update, SessionProvider:{0}", (SessionProvider == null? "null": "OK")));
                UserModel user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);

                if (user == null)
                {
                    Logger.Info(string.Format("Error in UpdateContent.Update: {0}", "user session is null"));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
                JObject request = HttpProxy.GetRequestBody<JObject>();
                if (request == null)
                {
                    Logger.Info(string.Format("Error in UpdateContent.Update: {0}", "request object is null"));
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                string id = string.Empty;
                string data = string.Empty;
                string moduleName = string.Empty;
                if (request[CommonConst.CommonField.DISPLAY_ID] != null && 
                    request[CommonConst.CommonField.DATA]!=null &&
                    request[CommonConst.CommonField.MODULE_NAME] != null 

                    )
                {
                    id = request[CommonConst.CommonField.DISPLAY_ID].ToString();
                    data = request[CommonConst.CommonField.DATA].ToString();
                     moduleName= request[CommonConst.CommonField.MODULE_NAME].ToString();
                }
               

                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(data) || string.IsNullOrEmpty(moduleName))
                {
                    Logger.Error(string.Format("Error in UpdateContent.Update: {0}", " id/data/moduleName empty"));
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                Logger.Debug("Get   data for UpdateContent.Update", request);

                JObject updateFilter = new JObject();
                updateFilter[CommonConst.CommonField.DISPLAY_ID] = id;
                if (moduleName != _contentUpdateModuleName)
                {
                    var originalData = DBProxy.FirstOrDefault(CommonConst.Collection.STATIC_CONTECT, updateFilter.ToString());
                    if (originalData == null)
                    {
                        Logger.Error(string.Format("Error in UpdateContent.Update: {0}, collection{1}, filter {2}", "originalData is null", CommonConst.Collection.STATIC_CONTECT, updateFilter.ToString()));
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                    //bool isOverride = true;
                    //bool.TryParse(originalData[CommonConst.CommonField.IS_OVERRIDE].ToString(), out isOverride);
                    //if (isOverride)
                    //{
                    //    Logger.Error(string.Format("Error in UpdateContent.Update: {0}", "isOverride parameter is true in the oroginal data"));
                    //    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    //}
                    originalData[CommonConst.CommonField.IS_OVERRIDE] = true;
                    originalData[CommonConst.CommonField.OVERRIDE_BY] = _contentUpdateModuleName;

                    if (DBProxy.Update(CommonConst.Collection.STATIC_CONTECT, updateFilter.ToString(), originalData) != 1)
                    {
                        Logger.Error(string.Format("Error in UpdateContent.Update: {0}", "error updating originalData"));
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }

                    originalData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                    originalData[CommonConst.CommonField.IS_OVERRIDE] = false;
                    originalData[CommonConst.CommonField.OVERRIDE_BY] = _contentUpdateModuleName;
                    originalData[CommonConst.CommonField.DATA] = data;
                    originalData[_contentHistory] = new JArray();

                    JObject filterFindExistingCustomization = new JObject();
                    filterFindExistingCustomization[CommonConst.CommonField.MODULE_NAME] = _contentUpdateModuleName;
                    filterFindExistingCustomization[CommonConst.CommonField.FILE_PATH] = originalData[CommonConst.CommonField.FILE_PATH].ToString();
                    var existingData = DBProxy.FirstOrDefault(CommonConst.Collection.STATIC_CONTECT, filterFindExistingCustomization.ToString());
                    if (existingData == null)
                    {
                        if (!DBProxy.Write(CommonConst.Collection.STATIC_CONTECT, originalData))
                        {
                            Logger.Error(string.Format("Error in UpdateContent.Update: {0}", "error adding  new data"));
                            return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                        }
                    }
                    else
                    {
                        if(!UpdateCustomContentData(existingData, user, data, filterFindExistingCustomization))
                        {
                            return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                        }
                    }
                    return ReturnSuccess(originalData);
                }
                else
                {
                    var contentCustomData = DBProxy.FirstOrDefault(CommonConst.Collection.STATIC_CONTECT, updateFilter.ToString());
                    if (contentCustomData == null)
                    {
                        Logger.Error(string.Format("Error in UpdateContent.Update: {0}", "contentCustomData  is null"));
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                    if (!UpdateCustomContentData(contentCustomData, user, data, updateFilter))
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                    return ReturnSuccess(contentCustomData);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error in  UpdateContent.Update: {0}", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        JObject ReturnSuccess(JObject data)
        {
            var file_path = data[CommonConst.CommonField.FILE_PATH].ToString();
            if(CommonUtility.IsServerSidePage(file_path))
            {
                HttpProxy.UnloadAppDomain();
            }
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
        }

        private bool UpdateCustomContentData(JObject contentCustomData, UserModel user,string data, JObject updateFilter)
        {
            JObject updateFilterExistingData = new JObject();
            updateFilterExistingData[CommonConst.CommonField.FILE_PATH] = contentCustomData[CommonConst.CommonField.FILE_PATH].ToString();
            updateFilterExistingData[CommonConst.CommonField.IS_OVERRIDE] = false;

            var updateData = new JObject();
            updateData[CommonConst.CommonField.OVERRIDE_BY] = _contentUpdateModuleName;
            updateData[CommonConst.CommonField.IS_OVERRIDE] = true;

            DBProxy.Update(CommonConst.Collection.STATIC_CONTECT, updateFilterExistingData.ToString(), updateData);

            var existingData = contentCustomData[CommonConst.CommonField.DATA];
            JObject history = new JObject();
            history[CommonConst.CommonField.DATA] = existingData;
            history[CommonConst.CommonField.UPDATED_BY] = user.user_id;
            history[CommonConst.CommonField.UPDATED_DATE_TIME] = CommonUtility.GetUnixTimestamp(DateTime.Now);
            (contentCustomData[_contentHistory] as JArray).Add(history);
            contentCustomData[CommonConst.CommonField.DATA] = data;
            if (DBProxy.Update(CommonConst.Collection.STATIC_CONTECT, updateFilter.ToString(), contentCustomData) != 1)
            {
                Logger.Error(string.Format("Error in UpdateContent.Update: {0}", "error updating contentCustomData"));
                return false;
            }
            return true;
        }
    }
}
