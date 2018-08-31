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
using ZNxtApp.Core.Interfaces;
using MyPhotos.Model;

namespace MyPhotos.Services.Api
{
    public class AlbumCtrl : ViewBaseService
    {
        IHttpFileUploader _fileUploader;
        public AlbumCtrl(ParamContainer paramContainer)
            : base(paramContainer)
        {
            _fileUploader = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
        }
        public JObject DeleteImage()
        {
            try
            {
                var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);
                var fileHash = HttpProxy.GetQueryString(ImageProcessor.FILE_HASH);


                var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (string.IsNullOrEmpty(galleryId) || string.IsNullOrEmpty(galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                if (!ImageGalleryHelper.IsOwner(DBProxy, SessionProvider, galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                var filter = new JObject();
                filter[CommonConst.CommonField.DISPLAY_ID] = galleryId;
                var galleryData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());
                if (galleryData == null)
                {
                    Logger.Error(string.Format("Gallery Not found :  Gallery: {0} ", galleryId));

                    return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
                }
                if (galleryData[ImageProcessor.FILE_HASHS] == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
                }
                var filehashData = (galleryData[ImageProcessor.FILE_HASHS] as JArray).FirstOrDefault(f => f.ToString() == fileHash);
                if (filehashData!=null)
                {
                    (galleryData[ImageProcessor.FILE_HASHS] as JArray).Remove(filehashData);
                    DBProxy.Update(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), galleryData, false, MergeArrayHandling.Replace);
                    JObject responseData = new JObject();
                    responseData[ImageProcessor.FILE_HASH] = fileHash;
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, responseData);

                }
                else
                {
                    Logger.Error(string.Format("File Hash Not found : FileHash: {0} Gallery: {1} ",fileHash, galleryId));
                    return ResponseBuilder.CreateReponse(CommonConst._404_RESOURCE_NOT_FOUND);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        public JObject AddImage()
        {
            try
            {
                var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);
                var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);


                if (string.IsNullOrEmpty(galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                if (!ImageGalleryHelper.IsOwner(DBProxy, SessionProvider, galleryId))
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }
                Logger.Debug(string.Format("File Count {0}", _fileUploader.GetFiles().Count));
                if (_fileUploader.GetFiles().Count > 0)
                {
                    FileInfo fi = new FileInfo(_fileUploader.GetFiles()[0]);
                    Logger.Debug(string.Format("Getting File Data"));

                    byte[] fileData = _fileUploader.GetFileData(_fileUploader.GetFiles()[0]);
                    using (Bitmap image = ImageGalleryHelper.GetImageBitmapFromByte(fileData))
                    {
                        var fileModel = new FileModel() { file_hash = Hashing.GetFileHash(fileData), file_paths = new List<string>() };
                        JObject fileFilter = new JObject();
                        fileFilter[ImageProcessor.FILE_HASH] = fileModel.file_hash;
                        if (DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_COLLECTION, fileFilter.ToString()) == null)
                        {
                            var imageJObjectData = ImageGalleryHelper.CreateFileDataJObject(fileModel, string.Empty, image);
                            imageJObjectData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                            imageJObjectData[ImageProcessor.OWNER] = user.user_id;
                            DBProxy.Write(ImageProcessor.MYPHOTO_COLLECTION, imageJObjectData);
                        }

                        var filter = new JObject();
                        filter[CommonConst.CommonField.DISPLAY_ID] = galleryId;

                        var galleryData = DBProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());
                        if (galleryData == null)
                        {
                            galleryData = new JObject();
                            galleryData[ImageProcessor.FILE_HASHS] = new JArray();
                            (galleryData[ImageProcessor.FILE_HASHS] as JArray).Add(fileModel.file_hash);
                            galleryData[ImageProcessor.GALLERY_THUMBNAIL] = fileModel.file_hash;
                            galleryData[ImageProcessor.OWNER] = user.user_id;
                            galleryData[ImageProcessor.AUTH_USERS] = new JArray();
                            (galleryData[ImageProcessor.AUTH_USERS] as JArray).Add(user.user_id);
                        }
                        else
                        {
                            (galleryData[ImageProcessor.FILE_HASHS] as JArray).Insert(0, fileModel.file_hash);
                        }
                        galleryData[ImageProcessor.FILES_COUNT] = (galleryData[ImageProcessor.FILE_HASHS] as JArray).Count;

                        DBProxy.Update(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), galleryData, false, MergeArrayHandling.Replace);
                        JObject responseData = new JObject();
                        responseData[ImageProcessor.FILE_HASH] = fileModel.file_hash;                        
                        return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, responseData);
                    }
                }
                else
                {
                    Logger.Error("no File found");
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
        public JObject Create()
        {
            try
            {
                var user = SessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
                if (user == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._401_UNAUTHORIZED);
                }

                var requestBody = HttpProxy.GetRequestBody<JObject>();
                if (requestBody == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }
                if (requestBody[ImageProcessor.DISPLAY_NAME] == null || requestBody[ImageProcessor.DESCRIPTION] == null || requestBody[ImageProcessor.AUTH_USERS] == null)
                {
                    return ResponseBuilder.CreateReponse(CommonConst._400_BAD_REQUEST);
                }

                JObject newAblum = new JObject();
                newAblum[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
                newAblum[CommonConst.CommonField.NAME] = string.Format("{0}-{1}", user.name, newAblum[CommonConst.CommonField.DISPLAY_ID].ToString());
                newAblum[ImageProcessor.DISPLAY_NAME] = requestBody[ImageProcessor.DISPLAY_NAME].ToString();
                newAblum[ImageProcessor.FILES_COUNT] = 0;
                newAblum[ImageProcessor.DESCRIPTION] = requestBody[ImageProcessor.DESCRIPTION].ToString();
                newAblum[ImageProcessor.AUTH_USERS] = new JArray();
                foreach (var e in (requestBody[ImageProcessor.AUTH_USERS] as JArray))
                {
                    (newAblum[ImageProcessor.AUTH_USERS] as JArray).Add(e.ToString());
                }
                newAblum[ImageProcessor.OWNER] = user.user_id;
                DBProxy.Write(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, newAblum);
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, newAblum);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
        public JObject UpdateAlbum()
        {
            try
            {

            var galleryId = HttpProxy.GetQueryString(ImageProcessor.GALLERY_ID);

            var requestBody = HttpProxy.GetRequestBody<JObject>();
            if (requestBody == null || string.IsNullOrEmpty(galleryId))
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
            if (requestBody[ImageProcessor.DISPLAY_NAME] != null)
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

            if (DBProxy.Update(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString(), data, false, MergeArrayHandling.Replace) != 1)
            {
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
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
