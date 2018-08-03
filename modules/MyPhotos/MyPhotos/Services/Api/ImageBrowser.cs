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
            try
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
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }

        private JObject GetGalleryImages(string id)
        {

            JObject filter = new JObject();
            filter[CommonConst.CommonField.DISPLAY_ID] = id;

            var data = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());

            if (data != null)
            {
                if (IsValidaUser(data))
                {
                    data.Remove(ImageProcessor.AUTH_USERS);
                    AddGalleryThumbnailImage(data);
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, GetGalleryPageData(data));
                }
                else
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
            }

        }

        JObject GetGalleryPageData(JObject data)
        {
            string strPagesize = HttpProxy.GetQueryString(CommonConst.CommonField.PAGE_SIZE_KEY.ToLower());
            string strCurrentPage = HttpProxy.GetQueryString(CommonConst.CommonField.CURRENT_PAGE_KEY.ToLower());

            int pageSize = 50;
            int currentPage = 0;

            int.TryParse(strPagesize, out pageSize);
            int.TryParse(strCurrentPage, out currentPage);

            int startCount = (pageSize * (currentPage));
            int endCount = startCount + pageSize;

            var fileHashs = (data[ImageProcessor.FILE_HASHS] as JArray);
             int totalPages  = 0;
             if (fileHashs.Count > 0 && pageSize != 0)
             {
                 totalPages = fileHashs.Count / pageSize;
             }
 
            var images = new JArray();
            for (int i = startCount; i < endCount && i < fileHashs.Count; i++)
            {
                var fileData = GetImageData(fileHashs[i].ToString());
                if (fileData.Count == 0)
                {
                    Logger.Error(string.Format("Image File not found {0}", fileHashs[i].ToString()));
                }
                else
                {
                    images.Add(fileData[0]);
                }
            }
            data.Remove(ImageProcessor.FILE_HASHS);
            data[ImageProcessor.IMAGES] = images;
            data[CommonConst.CommonField.PAGE_SIZE_KEY] = pageSize;
            data[CommonConst.CommonField.TOTAL_RECORD_COUNT_KEY] = fileHashs.Count;
            data[CommonConst.CommonField.TOTAL_PAGES_KEY] = totalPages;
            data[CommonConst.CommonField.CURRENT_PAGE_KEY] = currentPage;
            
            return data; 
        }

        private JObject GetAllGalleries()
        {

            var sort = new Dictionary<string, int>();
            sort[CommonConst.CommonField.NAME] = 1;
            sort[CommonConst.CommonField.ID] = 1;
            var response = GetPaggedData(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, null, null, sort, new List<string> { CommonConst.CommonField.DISPLAY_ID, CommonConst.CommonField.NAME, ImageProcessor.FILES_COUNT, ImageProcessor.GALLERY_THUMBNAIL, ImageProcessor.AUTH_USERS });
            List<JToken> filterData = new List<JToken>();

            foreach (var item in response[CommonConst.CommonField.DATA])
            {
                if (!IsValidaUser(item))
                {
                    filterData.Add(item);
                }
                else
                {
                    AddGalleryThumbnailImage(item);
                }
            }

            foreach (var item in filterData)
            {
                (response[CommonConst.CommonField.DATA] as JArray).Remove(item);
            }
            return response;
        }

        private void AddGalleryThumbnailImage(JToken galleryItem)
        {
            galleryItem[ImageProcessor.GALLERY_THUMBNAIL_IMAGE] = GetImageData(galleryItem[ImageProcessor.GALLERY_THUMBNAIL].ToString())[0];
            (galleryItem as JObject).Remove(ImageProcessor.GALLERY_THUMBNAIL);

        }
        public JObject GetUser()
        {
            var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);

            if (user == null)
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }
            else
            {
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(user)));
            }
        }

        private List<string> GetCurrentAuthGroups()
        {
            var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            var auth_users = new List<String> { "*" };
            if (user != null)
            {
                auth_users.Add(user.user_id);
                auth_users.AddRange(user.groups);
            }
            return auth_users;
        }

        private bool IsValidaUser(JToken GalleryData)
        {
            var auth_users = GetCurrentAuthGroups();
            bool isValid = false;
            foreach (var auth in GalleryData[ImageProcessor.AUTH_USERS])
            {
                if (auth_users.IndexOf(auth.ToString()) != -1)
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;

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
                else
                {

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
            var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);
            if (string.IsNullOrEmpty(galleryId))
            {
                return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
            }
            var galleryImages = GetGalleryImages(galleryId);
            if (galleryImages[CommonConst.CommonField.HTTP_RESPONE_CODE].ToString() != "1")
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }


            var data = GetImageData(fileHash, new List<string> { ImageProcessor.METADATA, ImageProcessor.TAGS });

            if (data.Count == 0)
            {
                return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
            }
            else
            {
                JArray files = galleryImages[CommonConst.CommonField.DATA][ImageProcessor.IMAGES] as JArray;
                data[0][ImageProcessor.RELATED_FILES] = new JArray();
                for (int i = 0; i < 10 && i < files.Count; i++)
                {
                    (data[0][ImageProcessor.RELATED_FILES] as JArray).Add(files[GetRandomNumberInRange(1, (files.Count - 1))]);
                }

                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data[0]);
            }
        }


        private JArray GetImageData(string fileHash, List<string> extraFields = null)
        {
            JObject filter = new JObject();
            filter[ImageProcessor.FILE_HASH] = fileHash;

            var fields = new List<string> { 
            ImageProcessor.FILE_HASH, 
            ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP, 
            ImageProcessor.PHOTO_DATE_TAKEN, 
            ImageProcessor.IMAGE_S_SIZE, 
            ImageProcessor.IMAGE_M_SIZE, 
            ImageProcessor.IMAGE_L_SIZE,
           
            ImageProcessor.CHANGESET_NO
            };
            if (extraFields != null) fields.AddRange(extraFields);

            var data = DBProxy.Get(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), fields);
            return data;
        }
        Random r = new Random();
        public int GetRandomNumberInRange(int minNumber, int maxNumber)
        {
            return Math.Abs(r.Next(minNumber, maxNumber));
        }

        private JObject GetAll()
        {
            var sort = new Dictionary<string, int>();
            sort[ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP] = -1;
            sort[CommonConst.CommonField.ID] = 1;
            return GetPaggedData(ImageProcessor.MYPHOTO_COLLECTION, null, null, sort, new List<string> { ImageProcessor.FILE_HASH, ImageProcessor.PHOTO_DATE_TAKEN_TIME_STAMP, ImageProcessor.IMAGE_S_SIZE, ImageProcessor.IMAGE_M_SIZE, ImageProcessor.IMAGE_L_SIZE });
        }

        public string GetAllMessages(Exception exp)
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
