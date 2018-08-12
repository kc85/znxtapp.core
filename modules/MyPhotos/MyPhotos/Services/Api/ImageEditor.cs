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
    public class ImageEditor : ViewBaseService
    {
        public ImageEditor(ParamContainer paramContainer)
            : base(paramContainer)
        {

        }

        public JObject Rotate()
        {
            try
            {

                var fileHash = HttpProxy.GetQueryString(ImageProcessor.FILE_HASH);
                var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);

                if (string.IsNullOrEmpty(fileHash) || string.IsNullOrEmpty(galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }

                if (!ImageGalleryHelper.IsOwner(DBProxy, SessionProvider, galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }

                var fileData = ImageGalleryHelper.GetImage(DBProxy, fileHash);

                if (fileHash == null)
                {
                    Logger.Error(string.Format("File not found ::{0}", fileHash));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }


                var path = string.Empty;
                var baseFolderPath = AppSettingService.GetAppSettingData("my_photo_path");
                if (string.IsNullOrEmpty(baseFolderPath)) throw new KeyNotFoundException("my_photo_path");
                if (fileData[ImageProcessor.FILE_PATHS] == null) throw new KeyNotFoundException(ImageProcessor.FILE_PATHS);

                Logger.Debug(string.Format("Getting file info BasePath  : {0}", baseFolderPath));
                foreach (var item in fileData[ImageProcessor.FILE_PATHS])
                {
                    if (File.Exists(string.Concat(baseFolderPath, "\\", item.ToString())))
                    {
                        path = string.Concat(baseFolderPath, "\\", item.ToString());
                        break;
                    }
                }
                if (!File.Exists(path)) throw new FileNotFoundException(path);

                var changesetNo = 0;
                if (fileData[ImageProcessor.CHANGESET_NO] != null)
                {
                    Logger.Debug(string.Format("changesetNo value from  fileData {0}", fileData[ImageProcessor.CHANGESET_NO].ToString()));
                    int.TryParse(fileData[ImageProcessor.CHANGESET_NO].ToString(), out changesetNo);
                }
                else
                {
                    Logger.Debug(string.Format("changesetNo is null in fileData"));
                }

                Logger.Debug(string.Format("Image  file path info BasePath  : {0} File Name :{1}", baseFolderPath, path));
              
                using (var image = ImageGalleryHelper.GetImageBitmapFromFile(path))
                {
                    Logger.Debug(string.Format("Processing Image BasePath  : {0} File Name :{1}", baseFolderPath, path));

                    int rotate = 90;
                    if (fileData[ImageProcessor.IMAGE_ROTATE] != null && int.TryParse(fileData[ImageProcessor.IMAGE_ROTATE].ToString(), out rotate))
                    {
                        rotate += 90;
                        if (rotate >= 360)
                        {
                            rotate = 0;
                        }
                    }
                    switch (rotate)
                    {
                        case 90:
                            ImageGalleryHelper.ProcessImage(fileData, image, RotateFlipType.Rotate90FlipNone);
                            Logger.Debug(string.Format("Rotate image to {0}. {1}", rotate, RotateFlipType.Rotate90FlipNone.ToString()));
                            break;
                        case 180:
                            ImageGalleryHelper.ProcessImage(fileData, image, RotateFlipType.Rotate180FlipNone);
                            Logger.Debug(string.Format("Rotate image to {0}. {1}", rotate, RotateFlipType.Rotate180FlipNone.ToString()));
                            break;
                        case 270:
                            ImageGalleryHelper.ProcessImage(fileData, image, RotateFlipType.Rotate270FlipNone);
                            Logger.Debug(string.Format("Rotate image to {0}. {1}", rotate, RotateFlipType.Rotate270FlipNone.ToString()));
                            break;
                        default:
                             ImageGalleryHelper.ProcessImage(fileData, image, RotateFlipType.RotateNoneFlipNone);
                             Logger.Debug(string.Format("Rotate image to {0}. {1}", rotate, RotateFlipType.RotateNoneFlipNone.ToString()));
                            break;
                    }
                    
                    fileData[ImageProcessor.IMAGE_ROTATE] = rotate;
                }
               
                Logger.Debug(string.Format("changesetNo : {0}. FileHash: {1}", changesetNo, fileHash));

                fileData[ImageProcessor.CHANGESET_NO] = (changesetNo+1);
 
                JObject filter = new JObject();
                filter[ImageProcessor.FILE_HASH] = fileHash;
                DBProxy.Update(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), fileData,true, MergeArrayHandling.Replace);

                fileData.Remove(ImageProcessor.IMAGE_L_BASE64); 
                fileData.Remove(ImageProcessor.IMAGE_M_BASE64);
                fileData.Remove(ImageProcessor.IMAGE_S_BASE64);

                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, fileData);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

        public JObject UpdateAlbum()
        {
            var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);

            var requestBody = HttpProxy.GetRequestBody<JObject>();
            if (requestBody ==null || string.IsNullOrEmpty(galleryId))
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
            if (!ImageGalleryHelper.IsOwner(DBProxy, SessionProvider, galleryId))
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }
            var filter = new JObject();
            filter[CommonConst.CommonField.DISPLAY_ID] = galleryId;

            var data = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());

            if (data == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
            if (requestBody[ImageProcessor.DISPLAY_NAME]!=null)
            {
                data[ImageProcessor.DISPLAY_NAME] = requestBody[ImageProcessor.DISPLAY_NAME].ToString();
            }
            if (requestBody[ImageProcessor.DESCRIPTION] != null)
            {
                data[ImageProcessor.DESCRIPTION] = requestBody[ImageProcessor.DESCRIPTION].ToString();
            }
            if (requestBody[ImageProcessor.GALLERY_THUMBNAIL] != null)
            {
                data[ImageProcessor.GALLERY_THUMBNAIL] = requestBody[ImageProcessor.GALLERY_THUMBNAIL].ToString();
            }
            if (requestBody[ImageProcessor.AUTH_USERS] != null)
            {
                data[ImageProcessor.AUTH_USERS] = requestBody[ImageProcessor.AUTH_USERS];
            }

            if (DBProxy.Update(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), data,false, MergeArrayHandling.Replace) != 1)
            {
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);

        }
    }
}
