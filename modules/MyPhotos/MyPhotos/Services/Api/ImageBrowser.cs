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
        ParamContainer _paramContainer;
        public ImageBrowser(ParamContainer paramContainer)
            : base(paramContainer)
        {
            _paramContainer = paramContainer;
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
                    var base64 = string.Empty;
                    if (data[0][imageType] != null)
                    {
                        base64 = data[0][imageType].ToString();
                    }
                    else
                    {
                        try
                        {
                            base64 = KeyValueStorage.Get<string>(ImageProcessor.IMAGE_KEY_VALUE_BUCKET, ImageProcessor.GetFileKey(imageType, fileHash));
                        }
                        catch (KeyNotFoundException ex)
                        {
                            Logger.Error(ex.Message, ex);
                        }
                    }
                    if (string.IsNullOrEmpty(base64))
                    {
                        HttpProxy.SetResponse(404);
                        return null;
                    }
                    HttpProxy.ResponseHeaders["Cache-Control"] = "public, max-age=172800";
                    return System.Convert.FromBase64String(base64);
                }
            }
            catch (Exception ex)
            {
                HttpProxy.SetResponse(500);
                Logger.Error(ex.Message, ex);
                return null;
            }
        }


        public JObject MyPhotoUsers()
        {
            try
            {
                JObject filter = new JObject();
                var data = DBProxy.Get(CommonConst.Collection.USERS, filter.ToString(), new List<string> { CommonConst.CommonField.USER_ID, CommonConst.CommonField.NAME, CommonConst.CommonField.USER_TYPE });
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        public JObject GetGalleryListByAdmin()
        {
            try
            {
                return GetPaggedData(ImageProcessor.MYPHOTO_GALLERY_COLLECTION,null,null,null, new List<string> { CommonConst.CommonField.DISPLAY_ID, ImageProcessor.DISPLAY_NAME, ImageProcessor.DESCRIPTION, CommonConst.CommonField.NAME, ImageProcessor.FILES_COUNT, ImageProcessor.GALLERY_THUMBNAIL, ImageProcessor.AUTH_USERS, ImageProcessor.OWNER });
            }

            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }

        }
        public JObject GetGallery()
        {
            try
            {
                var id = HttpProxy.GetQueryString(CommonConst.CommonField.DISPLAY_ID);
                if (_paramContainer.GetKey(CommonConst.CommonField.DISPLAY_ID) != null)
                {
                    id = (string)_paramContainer.GetKey(CommonConst.CommonField.DISPLAY_ID);
                    Logger.Debug(string.Format("Filter on Gallery {0}", id));
;                }
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
                ImageGalleryHelper.AddDefaultOwner(data);
                if (ImageGalleryHelper.IsValidaUser(data, SessionProvider))
                {
                    AddGalleryThumbnailImage(data);

                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, GetGalleryPageData(data));
                }
                else
                {
                    data.Remove(ImageProcessor.AUTH_USERS);
                    data.Remove(ImageProcessor.FILE_HASHS);
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED, data);
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
            if (_paramContainer.GetKey(CommonConst.CommonField.PAGE_SIZE_KEY.ToLower()) != null)
            {
                strPagesize = (string)_paramContainer.GetKey(CommonConst.CommonField.PAGE_SIZE_KEY.ToLower());
            }
            if (_paramContainer.GetKey(CommonConst.CommonField.CURRENT_PAGE_KEY.ToLower()) != null)
            {
                strCurrentPage = (string)_paramContainer.GetKey(CommonConst.CommonField.CURRENT_PAGE_KEY.ToLower());
            }

            int pageSize = 50;
            int currentPage = 1;

            int.TryParse(strPagesize, out pageSize);
            int.TryParse(strCurrentPage, out currentPage);

            int startCount = (pageSize * (currentPage));
            int endCount = startCount + pageSize;
            if (data[ImageProcessor.FILE_HASHS] == null)
            {
                data[ImageProcessor.FILE_HASHS] = new JArray();
            }
            var fileHashs = (data[ImageProcessor.FILE_HASHS] as JArray);
             int totalPages  = 0;
             if (fileHashs.Count > 0 && pageSize != 0)
             {
                 totalPages = fileHashs.Count / pageSize;
             }
 
            var images = new JArray();
            for (int i = startCount; i < endCount && i < fileHashs.Count; i++)
            {
                var fileData = ImageGalleryHelper.GetImageData (DBProxy,SessionProvider, fileHashs[i].ToString());
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
            string filterKey = "filter";
            var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            var userId =  HttpProxy.GetQueryString(CommonConst.CommonField.USER_ID);

            if (_paramContainer.GetKey(CommonConst.CommonField.USER_ID) != null)
            {
                userId = (string)_paramContainer.GetKey(CommonConst.CommonField.USER_ID);
            }
            if (!string.IsNullOrEmpty(userId) && user != null &&  userId != user.user_id)
            {
                return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
            }

            int pagesize = 100;
            int currentpage = 1;
            string filter = "{}";
            if(!string.IsNullOrEmpty(HttpProxy.GetQueryString(CommonConst.CommonField.PAGE_SIZE_KEY)))
                int.TryParse(HttpProxy.GetQueryString(CommonConst.CommonField.PAGE_SIZE_KEY), out pagesize);

            if (!string.IsNullOrEmpty(HttpProxy.GetQueryString(CommonConst.CommonField.CURRENT_PAGE_KEY)))
                int.TryParse(HttpProxy.GetQueryString(CommonConst.CommonField.CURRENT_PAGE_KEY), out currentpage);

            if (!string.IsNullOrEmpty(HttpProxy.GetQueryString(filterKey)))
            {
                var filterValue = HttpProxy.GetQueryString(filterKey);
                filter = "{display_name:{'$regex' : '"+ filterValue + "', '$options' : 'i'}}";
                Logger.Debug(filter);
            }


            //var sort = new Dictionary<string, int>();
            //sort[CommonConst.CommonField.NAME] = 1;
            //sort[CommonConst.CommonField.ID] = 1;

            var response = GetPagedData(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter, new List<string> { CommonConst.CommonField.DISPLAY_ID, ImageProcessor.DISPLAY_NAME, ImageProcessor.DESCRIPTION, CommonConst.CommonField.NAME, ImageProcessor.FILES_COUNT, ImageProcessor.GALLERY_THUMBNAIL, ImageProcessor.AUTH_USERS, ImageProcessor.OWNER },null, pagesize, currentpage);

            List<JToken> filterData = new List<JToken>();
            foreach (var item in response[CommonConst.CommonField.DATA])
            {
               

                if (string.IsNullOrEmpty(userId))
                {
                    ImageGalleryHelper.AddDefaultOwner(item);
                    if (!ImageGalleryHelper.IsValidaUser(item, SessionProvider))
                    {
                        filterData.Add(item);
                    }
                    else
                    {
                       
                        if (item[ImageProcessor.OWNER] == null)
                        {
                            item[ImageProcessor.OWNER] = ImageProcessor.DEFAULT_OWNER;
                        }
                        AddGalleryThumbnailImage(item);
                    }
                }
                else
                {
                    if (item[ImageProcessor.OWNER]!=null && item[ImageProcessor.OWNER].ToString() == userId)
                    {
                        AddGalleryThumbnailImage(item);
                    }
                    else
                    {
                        filterData.Add(item);
                    }
                }
            }

            foreach (var item in filterData)
            {
                (response[CommonConst.CommonField.DATA] as JArray).Remove(item);
            }
            return response;
        }

        //private JObject GetGallery(string galleryid)
        //{
        //    JObject filter = new JObject();
        //    filter[ImageProcessor.GALLERY_ID] = galleryid;
        //    var data = DBProxy.Get(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), new List<string> { CommonConst.CommonField.DISPLAY_ID,
        //        CommonConst.CommonField.NAME, ImageProcessor.FILES_COUNT,
        //        ImageProcessor.GALLERY_THUMBNAIL,
        //        ImageProcessor.AUTH_USERS,
        //        ImageProcessor.OWNER,
        //    ImageProcessor.DESCRIPTION});
        //    if (data.Count == 1)
        //    {
        //        AddGalleryThumbnailImage(data.First());
        //        return data.First() as JObject;
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        private void AddGalleryThumbnailImage(JToken galleryItem)
        {
            if (galleryItem[ImageProcessor.DISPLAY_NAME] == null)
            {
                galleryItem[ImageProcessor.DISPLAY_NAME] = galleryItem[CommonConst.CommonField.NAME].ToString();
            }
            if (galleryItem[ImageProcessor.DESCRIPTION]==null)
            {
                galleryItem[ImageProcessor.DESCRIPTION] = string.Empty;
            }
            if (galleryItem[ImageProcessor.GALLERY_THUMBNAIL] != null)
            {
                galleryItem[ImageProcessor.GALLERY_THUMBNAIL_IMAGE] = ImageGalleryHelper.GetImageData(DBProxy, SessionProvider, galleryItem[ImageProcessor.GALLERY_THUMBNAIL].ToString())[0];
            }

        }
        

        //private List<string> GetCurrentAuthGroups()
        //{
        //    var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
        //    var auth_users = new List<String> { "*" };
        //    if (user != null)
        //    {
        //        auth_users.Add(user.user_id);
        //        auth_users.AddRange(user.groups);
        //    }
        //    return auth_users;
        //}

        //private bool IsValidaUser(JToken GalleryData)
        //{
        //    var auth_users = GetCurrentAuthGroups();
        //    bool isValid = false;
             

        //    foreach (var auth in GalleryData[ImageProcessor.AUTH_USERS])
        //    {
        //        if (auth_users.IndexOf(auth.ToString()) != -1)
        //        {
        //            isValid = true;
        //            break;
        //        }
        //    }
        //    return isValid;

        //}

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

            var data =ImageGalleryHelper.GetImageData(DBProxy,SessionProvider, fileHash, new List<string> { ImageProcessor.METADATA, ImageProcessor.TAGS, ImageProcessor.FILE_PATHS });

            if (data.Count == 0)
            {
                return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
            }
            else
            {
                AddToUserView(galleryId, fileHash);
                //JArray files = galleryImages[CommonConst.CommonField.DATA][ImageProcessor.IMAGES] as JArray;
                //data[0][ImageProcessor.RELATED_FILES] = new JArray();
                //for (int i = 0; i < 10 && i < files.Count; i++)
                //{
                //    (data[0][ImageProcessor.RELATED_FILES] as JArray).Add(files[GetRandomNumberInRange(1, (files.Count - 1))]);
                //}

                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data[0]);
            }
        }

        private void AddToUserView(string galleryId, string fileHash)
        {
            Logger.Debug("Adding UserViews");

            var filter = new JObject();
            filter[ImageProcessor.FILE_HASH] = fileHash;
            var viewData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_IMAGE_VIEW_COLLECTION, filter.ToString());

            if (viewData == null)
            {
                viewData = new JObject();
                viewData[ImageProcessor.USERS] = new JArray();
                viewData[ImageProcessor.COUNT] = 0;
                viewData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            }
            var viewCount = 0;
            viewData[ImageProcessor.FILE_HASH] = fileHash;
            Logger.Debug("Getting existing Count");
            if (viewData[ImageProcessor.COUNT]!=null)
            int.TryParse(viewData[ImageProcessor.COUNT].ToString(), out viewCount);

            viewData[ImageProcessor.COUNT] = viewCount + 1;
            Logger.Debug("Getting Session User ");
            var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            string userId = "-1";
            if (user != null) userId = user.user_id;

            Logger.Debug("Getting UserData", viewData);
            JToken userData = null;
            if (viewData[ImageProcessor.USERS] != null)
                userData = (viewData[ImageProcessor.USERS] as JArray).FirstOrDefault(f => { return f[CommonConst.CommonField.USER_ID] != null && f[CommonConst.CommonField.USER_ID].ToString() == userId; });
            if (userData == null)
            {
                userData = new JObject();
                userData[CommonConst.CommonField.USER_ID] = userId;
                userData[ImageProcessor.COUNT] = 0;
                if (viewData[ImageProcessor.USERS] == null)
                {
                    viewData[ImageProcessor.USERS] = new JArray();
                }
                (viewData[ImageProcessor.USERS] as JArray).Add(userData);
            }
            var userViewCount = 0;
            Logger.Debug("Getting existing user Count");
            if (userData[ImageProcessor.COUNT]!=null)
            int.TryParse(userData[ImageProcessor.COUNT].ToString(), out userViewCount);

            userData[ImageProcessor.COUNT] = userViewCount + 1;

            DBProxy.Update(ImageProcessor.MYPHOTO_IMAGE_VIEW_COLLECTION,filter.ToString(),viewData, true, MergeArrayHandling.Replace);
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
