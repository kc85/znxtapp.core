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

        public JObject GetGallery()
        {
            var id = HttpProxy.GetQueryString("id");
            if (string.IsNullOrEmpty(id))
            {
                return GetAllGalleries();
            }
            else
            {
                return GetGalleryImages(id);
            }
        }

        private JObject GetGalleryImages(string id)
        {
            JObject filter = new JObject();
            filter[CommonConst.CommonField.DISPLAY_ID] = id;

            var data = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());
            if (data != null)
            {
                data.Remove(ImageProcessor.AUTH_USERS);
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data);
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
            }
        }
        private JObject GetAllGalleries()
        {
            var sort = new Dictionary<string, int>();
            sort[CommonConst.CommonField.NAME] = 1;
            sort[CommonConst.CommonField.ID] = 1;
            return GetPaggedData(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, null, null, sort, new List<string> { CommonConst.CommonField.DISPLAY_ID, CommonConst.CommonField.NAME, ImageProcessor.FILES_COUNT, ImageProcessor.GALLERY_THUMBNAIL });
        }
        public JObject Get()
        {
            try
            {
                string fileHash = HttpProxy.GetQueryString(ImageProcessor.FILE_HASH);
                if (string.IsNullOrEmpty(fileHash))
                {
                    return GetAll();
                }
                else {

                    return GetFileInfo(fileHash);
                }
            
            }
            catch (Exception ex)
            {
                Logger.Error(GetAllMessages(ex), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

        private JObject GetFileInfo(string fileHash)
        {
            var galleryId = HttpProxy.GetQueryString("galleryid");
            if (string.IsNullOrEmpty(galleryId))
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }

            JObject filter = new JObject();
            filter[ImageProcessor.FILE_HASH] = fileHash;
            var data = DBProxy.Get(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), new List<string> { 
            ImageProcessor.FILE_HASH, 
            ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP, 
            ImageProcessor.PHOTO_DATE_TAKEN, 
            ImageProcessor.IMAGE_S_SIZE, 
            ImageProcessor.IMAGE_M_SIZE, 
            ImageProcessor.IMAGE_L_SIZE,
            ImageProcessor.METADATA,
            ImageProcessor.TAGS
            });
            

            if (data.Count == 0)
            {
                return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
            }
            else
            {
                var galleryImages = GetGalleryImages(galleryId);
                JArray files  = galleryImages[CommonConst.CommonField.DATA][ImageProcessor.FILE_HASHS] as JArray;
                data[0]["related_files"] = new JArray();
                for (int i = 0; i < 10 && i < files.Count; i++)
                {
                    (data[0]["related_files"] as JArray).Add(files[GetRandomNumberInRange(0, (files.Count - 1))].ToString());
                }

                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data[0]);
            }
        }
        Random r = new Random(); 
        public int  GetRandomNumberInRange(int minNumber, int maxNumber)
        {
            return Math.Abs(r.Next(minNumber,maxNumber));
        }

        private JObject GetAll()
        {
            var sort = new Dictionary<string, int>();
            sort[ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP] = -1;
            sort[CommonConst.CommonField.ID] = 1;
            return GetPaggedData(ImageProcessor.MYPHOTO_COLLECTION, null, null, sort, new List<string> { ImageProcessor.FILE_HASH, ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP, ImageProcessor.IMAGE_S_SIZE, ImageProcessor.IMAGE_M_SIZE, ImageProcessor.IMAGE_L_SIZE });
        }
        public string GetAllMessages( Exception exp)
        {
            string message = string.Empty;
            Exception innerException = exp;

            do
            {
                message = message + (string.IsNullOrEmpty(innerException.Message) ? string.Empty : innerException.Message);
                innerException = innerException.InnerException;
            }
            while (innerException != null);

            return message;
        }
    }
}
