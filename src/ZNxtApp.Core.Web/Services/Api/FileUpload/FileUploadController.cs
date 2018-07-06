using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Web.Services.Api.FileUpload
{
    public class FileUploadController : ApiBaseService
    {
        IHttpFileUploader _fileUploader;
        private ParamContainer _paramContainer;
        public FileUploadController(ParamContainer paramContainer) : base(paramContainer)
        {
            _fileUploader = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
            _paramContainer = paramContainer; 
        }
        public JObject Upload()
        {
            try
            {
                if (_fileUploader.GetFiles().Count == 0)
                {
                    Logger.Error("No File Found for upload");
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                string uploadCollection = _paramContainer.GetKey(CommonConst.CommonValue.COLLECTION);
                if (string.IsNullOrEmpty(uploadCollection))
                {
                    uploadCollection = CommonConst.Collection.STATIC_CONTECT;
                }
                string storegeType = _paramContainer.GetKey("storege_type");
                string baseFolder = _paramContainer.GetKey(CommonConst.CommonValue.BASE_PATH);

                if (storegeType.Trim().ToLower() == "file")
                {
                    var fileName = _fileUploader.GetFiles()[0];
                    string filePath = string.Format("{0}\\{1}", baseFolder, fileName);

                    var reponse = _fileUploader.Save(fileName, filePath);
                    if (!string.IsNullOrEmpty(reponse))
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                    }
                    else
                    {
                        Logger.Error("_fileUploader.Save fail");
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(baseFolder))
                    {
                        baseFolder = "/";
                    }

                    JObject fileResonse = _fileUploader.SaveToDB(DBProxy, _fileUploader.GetFiles()[0], baseFolder, uploadCollection);
                    if (fileResonse[CommonConst.CommonField.DATA] != null)
                    {
                        fileResonse.Remove(CommonConst.CommonField.DATA);
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, fileResonse);
                    }
                    else
                    {
                        Logger.Error("_fileUploader.SaveToDB fail");
                        return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR, fileResonse);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error in FileUploadController.Upload : {0} ", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
           
        }
    }
}
