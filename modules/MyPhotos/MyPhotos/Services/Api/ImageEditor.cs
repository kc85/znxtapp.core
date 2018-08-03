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

                if (!ImageGalleryHelper.HasAccess(DBProxy, SessionProvider, galleryId, fileHash))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }

                var fileData = ImageGalleryHelper.GetImage(DBProxy, fileHash);

                if (fileHash == null)
                {
                    Logger.Error(string.Format("File not found ::{0}", fileHash));
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
                var changesetNo = 0;


                fileData[ImageProcessor.IMAGE_L_BASE64] = ImageGalleryHelper.RotateImage(fileData[ImageProcessor.IMAGE_L_BASE64].ToString(),70);
                fileData[ImageProcessor.IMAGE_M_BASE64] = ImageGalleryHelper.RotateImage(fileData[ImageProcessor.IMAGE_M_BASE64].ToString(),90);
                fileData[ImageProcessor.IMAGE_S_BASE64] = ImageGalleryHelper.RotateImage(fileData[ImageProcessor.IMAGE_S_BASE64].ToString(),90);
                
                if (fileData[ImageProcessor.CHANGESET_NO]!=null) int.TryParse(fileData[ImageProcessor.CHANGESET_NO].ToString(), out changesetNo);
                
                fileData[ImageProcessor.CHANGESET_NO] = (changesetNo+1);
 
                JObject filter = new JObject();
                filter[ImageProcessor.FILE_HASH] = fileHash;
                DBProxy.Update(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), fileData, false, MergeArrayHandling.Replace);
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

        public JObject UpdateAlbumCover()
        {
            var fileHash = HttpProxy.GetQueryString(ImageProcessor.FILE_HASH);
            var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);

            if (string.IsNullOrEmpty(fileHash) || string.IsNullOrEmpty(galleryId))
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
            if (!ImageGalleryHelper.HasAccess(DBProxy, SessionProvider, galleryId, fileHash))
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }
            var filter = new JObject();
            filter[CommonConst.CommonField.DISPLAY_ID] = galleryId;
            var data = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());
            data[ImageProcessor.GALLERY_THUMBNAIL] = fileHash;
            if (DBProxy.Update(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), data) != 1)
            {
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);

        }
    }
}
