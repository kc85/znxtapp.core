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
using MyPhotos.Model;

namespace MyPhotos.Services.Api
{
    public class ImageBackgroundSync : CronServiceBase
    {
        const string CRON_JOB = "_param_cron_job_object";
        ParamContainer _paramContainer;
        public ImageBackgroundSync(ParamContainer paramContainer)
            : base(paramContainer)
        {
            _paramContainer = paramContainer;
        }

        public int  Sync()
        {
            try
            {
                Logger.Debug("Calling CronJob ImageBackgroundSync");

                JObject crobJob = _paramContainer.GetKey(CRON_JOB);

                var file_path = AppSettingService.GetAppSettingData("my_photo_path");
                Logger.Debug("Calling CronJob ImageBackgroundSync FilePath " + file_path);

                if (string.IsNullOrEmpty(file_path))
                {
                    return 0;
                }
                ImageProcessor ip = new ImageProcessor();
                ip.Scan(file_path,DBProxy,(string mesage)=>{ 
                    
                    Logger.Debug(string.Format("ImageBackgroundSync: {0}" , mesage));
                    return true;
                
                });

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return 0;
            }
        }
    }
}
