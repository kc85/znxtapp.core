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

namespace MyPhotos.Services.Api
{
    public class ImageBrowser : ViewBaseService
    {
        public ImageBrowser(ParamContainer paramContainer)
            : base(paramContainer)
        {

        }

        public byte[] GetImage()
        {
            try
            {
                var fileHash = HttpProxy.GetQueryString(ImageProcessor.FILE_HASH);
                var type = HttpProxy.GetQueryString("t");

                if (string.IsNullOrEmpty(fileHash))
                {
                    HttpProxy.SetResponse(400);
                }
                if (string.IsNullOrEmpty(type))
                {
                    type = "s";
                }
                var imageType = ImageProcessor.IMAGE_S_BASE64;
                switch (type.Trim().ToLower())
                {
                    case "l":
                        imageType = ImageProcessor.IMAGE_L_BASE64;
                        break;
                    case "m":
                        imageType = ImageProcessor.IMAGE_M_BASE64;
                        break;
                }

                JObject filter = new JObject();
                filter[ImageProcessor.FILE_HASH] = fileHash;
                var data = DBProxy.Get(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), new List<string> { imageType });
                if (data.Count == 0)
                {
                    Logger.Error("Data not found");
                    HttpProxy.SetResponse(404);
                    return null;
                }
                else
                {
                    var base64 = data[0][imageType].ToString();
                    HttpProxy.ResponseHeaders["Cache-Control"] = "public, max-age=172800";
                    return System.Convert.FromBase64String(base64);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return null;
            }
        }

        public JObject Get()
        {
            try
            {
                var sort = new Dictionary<string, int>();
                sort[ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP] = -1;
                return GetPaggedData(ImageProcessor.MYPHOTO_COLLECTION, null, null, sort, new List<string> { ImageProcessor.FILE_HASH, ImageProcessor.FILE_PATHS, ImageProcessor.TAGS, ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP, ImageProcessor.PHOTO_DATE_TAKEN, ImageProcessor.METADATA });
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }
    }
}
