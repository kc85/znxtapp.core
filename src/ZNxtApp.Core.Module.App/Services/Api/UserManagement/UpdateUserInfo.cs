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
using ZNxtApp.Core.Interfaces;
using System.IO;

namespace ZNxtApp.Core.Module.App.Services.Api.UserManagement
{
    public class UpdateUserInfo : ViewBaseService
    {
        IHttpFileUploader _fileUploader;
        private ParamContainer _paramContainer;

        public UpdateUserInfo(ParamContainer paramContainer) : base(paramContainer)
        {
            _fileUploader = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
            _paramContainer = paramContainer;
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

        public JObject AdminChangeUserImage()
        {
            var user_id = HttpProxy.GetQueryString(CommonConst.CommonField.USER_ID);
            if (string.IsNullOrEmpty(user_id))
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            Logger.Debug(string.Format("File Count {0}", _fileUploader.GetFiles().Count));
            if (_fileUploader.GetFiles().Count > 0)
            {
                var path = string.Format("/frontend/{0}/{1}", "userpic", user_id);

                FileInfo fi = new FileInfo(_fileUploader.GetFiles()[0]);
                Logger.Debug(string.Format("Getting File Data"));

                byte[] fileData = _fileUploader.GetFileData(_fileUploader.GetFiles()[0]);

                
                JObject uploadReponse = _fileUploader.SaveToDB(DBProxy, fi.Name, path, CommonConst.Collection.STATIC_CONTECT, null, Convert.ToBase64String(fileData));
                Logger.Debug(string.Format("Uploaded default file"), uploadReponse);

                JObject uploadReponseS = _fileUploader.SaveToDB(DBProxy, fi.Name, path, CommonConst.Collection.STATIC_CONTECT,null, ImageUtility.GetSmallImage(fileData));
                Logger.Debug(string.Format("Uploaded small file"), uploadReponseS);

                JObject uploadReponseM = _fileUploader.SaveToDB(DBProxy, fi.Name, path, CommonConst.Collection.STATIC_CONTECT, null, ImageUtility.GetMediumImage(fileData));
                Logger.Debug(string.Format("Uploaded medium file"), uploadReponseM);


                JObject uploadReponseL = _fileUploader.SaveToDB(DBProxy, fi.Name, path, CommonConst.Collection.STATIC_CONTECT, null, ImageUtility.GetLargeImage(fileData));
                Logger.Debug(string.Format("Uploaded large file"), uploadReponseL);


                if (uploadReponse != null && uploadReponseS != null && uploadReponseM!=null && uploadReponseL!=null)
                {
                    var filePath = uploadReponse[CommonConst.CommonField.FILE_PATH].ToString();
                    var filePathS = uploadReponseS[CommonConst.CommonField.FILE_PATH].ToString();
                    var filePathM = uploadReponseM[CommonConst.CommonField.FILE_PATH].ToString();
                    var filePathL = uploadReponseL[CommonConst.CommonField.FILE_PATH].ToString();

                    JObject updateFilter = new JObject();
                    updateFilter[CommonConst.CommonField.USER_ID] = user_id;

                    JObject userdata = new JObject();
                    userdata[ModuleAppConsts.Field.USER_PIC] = filePath;
                    userdata[ModuleAppConsts.Field.USER_PIC_S] = filePathS;
                    userdata[ModuleAppConsts.Field.USER_PIC_M] = filePathM;
                    userdata[ModuleAppConsts.Field.USER_PIC_L] = filePathL;
                    userdata[CommonConst.CommonField.USER_ID] = user_id;
                    Logger.Debug("Updating User AdminChangeUserImage", userdata);
                    var updateCount = DBProxy.Update(CommonConst.Collection.USER_INFO, updateFilter.ToString(), userdata, true);
                    Logger.Debug(string.Format("AdminChangeUserImage UpdateCount:{0}", userdata));

                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, userdata);

                }
                else
                {
                    Logger.Error(string.Format("Error while uploading image AdminChangeUserImage"));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR, uploadReponse);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
        }
    }
}
