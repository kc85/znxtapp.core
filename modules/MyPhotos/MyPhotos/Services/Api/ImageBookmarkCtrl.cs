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
    public class ImageBookmarkCtrl : ViewBaseService
    {
        public ImageBookmarkCtrl(ParamContainer paramContainer)
            : base(paramContainer)
        {

        }

        public JObject Bookmark()
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
                var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);

                var filter = new JObject();
                filter[CommonConst.CommonField.USER_ID] = user.user_id;

                var bookmarkData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_IMAGE_BOOKMARK_COLLECTION, filter.ToString());
                if (bookmarkData == null)
                {
                    bookmarkData = new JObject();
                     bookmarkData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                    bookmarkData[CommonConst.CommonField.USER_ID] = user.user_id;
                    bookmarkData[ImageProcessor.IMAGES] = new JArray();
                }

                JObject bookmarrkResponse = new JObject();
                var bookmarkFile = (bookmarkData[ImageProcessor.IMAGES] as JArray).FirstOrDefault(f => f[ImageProcessor.FILE_HASH].ToString() == fileHash);
                Logger.Debug("Get Bookmark data");
                if (bookmarkFile != null)
                { 
                    (bookmarkData[ImageProcessor.IMAGES] as JArray).Remove(bookmarkFile);                    
                    bookmarrkResponse[ImageProcessor.COUNT] = -1;
                    Logger.Debug("Removed");
                }
                else
                {
                    
                    bookmarkFile = new JObject();
                    bookmarkFile[ImageProcessor.FILE_HASH] = fileHash;
                    bookmarkFile[ImageProcessor.GALLERY_ID] = galleryId;
                    (bookmarkData[ImageProcessor.IMAGES] as JArray).Add(bookmarkFile);                    
                    bookmarrkResponse[ImageProcessor.COUNT] = +1;
                    Logger.Debug("Adding");
                }

                DBProxy.Update(ImageProcessor.MYPHOTO_IMAGE_BOOKMARK_COLLECTION, filter.ToString(), bookmarkData, true, MergeArrayHandling.Replace);

                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, bookmarrkResponse);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }
        public JObject GetUserBookmark()
        {
            var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            if (user == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }

            var filter = new JObject();
            filter[CommonConst.CommonField.USER_ID] = user.user_id;

            var bookmarkData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_IMAGE_BOOKMARK_COLLECTION, filter.ToString());
            JObject responseData = new JObject();
            if (bookmarkData != null)
            {
                JArray data = new JArray();
                foreach (var item in bookmarkData[ImageProcessor.IMAGES])
                {
                    var d = ImageGalleryHelper.GetImageData(DBProxy, SessionProvider, item[ImageProcessor.FILE_HASH].ToString()).First();
                    d[ImageProcessor.GALLERY_ID] = item[ImageProcessor.GALLERY_ID];
                    data.Add(d);
                }
                responseData[CommonConst.CommonField.NAME] = "My Bookmarked Images";
                responseData[ImageProcessor.IMAGES] = data;
            }
            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, responseData);
        }

       
    }
}
