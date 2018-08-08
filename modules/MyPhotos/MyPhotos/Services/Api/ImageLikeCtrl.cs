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
    public class ImageLikeCtrl : ViewBaseService
    {
        public ImageLikeCtrl(ParamContainer paramContainer)
            : base(paramContainer)
        {

        }

        public JObject Like()
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
                filter[ImageProcessor.FILE_HASH] = fileHash;

                var likeData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_IMAGE_LIKE_COLLECTION, filter.ToString());
                if (likeData == null)
                {
                    likeData = new JObject();
                    likeData[ImageProcessor.USERS] = new JArray();
                    likeData[ImageProcessor.FILE_HASH] = fileHash;
                    likeData[ImageProcessor.COUNT] = 0;
                    likeData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                }

                var totalCount = 0;
                int.TryParse(likeData[ImageProcessor.COUNT].ToString(), out totalCount);
                JObject like = new JObject();
                var likeUser = (likeData[ImageProcessor.USERS] as JArray).FirstOrDefault(f=>f.ToString() == user.user_id) ;
                if (likeUser != null)
                {
                    (likeData[ImageProcessor.USERS] as JArray).Remove(likeUser);
                    totalCount--;
                    like[ImageProcessor.COUNT] = -1; 
                }
                else
                {
                    (likeData[ImageProcessor.USERS] as JArray).Add(user.user_id);
                    totalCount++;
                    like[ImageProcessor.COUNT] = +1;
                }
                likeData[ImageProcessor.COUNT] = (likeData[ImageProcessor.USERS] as JArray).Count;

                DBProxy.Update(ImageProcessor.MYPHOTO_IMAGE_LIKE_COLLECTION, filter.ToString(), likeData, true, MergeArrayHandling.Replace);
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, like);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

       
    }
}
