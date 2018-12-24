using MyPhotos.Services.ImageService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Helpers;
using System.IO;
using System.Drawing;

namespace MyPhotos.Services.Api
{
    public class FolderScanCtrl : ViewBaseService
    {
        public FolderScanCtrl(ParamContainer paramContainer)
            : base(paramContainer)
        {

        }

        public JObject Scan()
        {
            try
            {
                var path = HttpProxy.GetQueryString(ImageProcessor.PATH);
           
                if (string.IsNullOrEmpty(path))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                var file_path = AppSettingService.GetAppSettingData("my_photo_path");

                if (string.IsNullOrEmpty(file_path))
                {
                    Logger.Error("No file path from App setting");
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
                
                ImageProcessor ip = new ImageProcessor();
                ip.Scan(file_path, path, DBProxy,KeyValueStorage, (string mesage) => {

                    Logger.Debug(string.Format("ImageBackgroundSync: {0}", mesage));
                    return true;

                });
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

       
    }
}
