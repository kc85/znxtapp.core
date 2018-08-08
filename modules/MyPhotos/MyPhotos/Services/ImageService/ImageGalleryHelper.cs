using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Helpers;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace MyPhotos.Services.ImageService
{
    public static class ImageGalleryHelper
    {
        public static Image GetImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

        //public static string RotateImage(string base64, int quality)
        //{
        //    string base64Data = "";
        //    using (Image img = GetImage(base64))
        //    {
        //        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                
        //        System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

        //        // Create an EncoderParameters object.

        //        EncoderParameters encoderParameters = new EncoderParameters(1);

        //        // Save the image as a JPEG file with quality level.

        //        EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);

        //        encoderParameters.Param[0] = encoderParameter;

        //        using (var ms = new MemoryStream())
        //        {
        //            img.Save(ms, ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid), encoderParameters);
        //            base64Data = Convert.ToBase64String(ms.ToArray());
        //        }

        //    }
        //    return base64Data;
        //}

        public static JObject GetImage(IDBService dbProxy, string fileHash)
        {
            JObject filter = new JObject();
            filter[ImageProcessor.FILE_HASH] = fileHash;
           return dbProxy.FirstOrDefault(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString());
        }
        public static bool HasAccess(IDBService dbProxy, ISessionProvider sessionProvider, string galleryId, string fileHash = null)
        {
            JObject filter = new JObject();
            filter[CommonConst.CommonField.DISPLAY_ID] = galleryId;

            var data = dbProxy.FirstOrDefault(ImageProcessor.MYPHOTO_GALLERY_COLLECTION, filter.ToString());

            var isValid = IsValidaUser(data, sessionProvider);
            if (isValid && !string.IsNullOrEmpty(fileHash))
            {
                isValid = (data[ImageProcessor.FILE_HASHS] as JArray).FirstOrDefault(f => f.ToString() == fileHash) != null;
            }
            return isValid;

        }
        private static bool IsValidaUser(JObject data, ISessionProvider sessionProvider)
        {
            if (data == null) return false;

            var auth_users = GetCurrentAuthGroups(sessionProvider);
            bool isValid = false;
            foreach (var auth in data[ImageProcessor.AUTH_USERS])
            {
                if (auth_users.IndexOf(auth.ToString()) != -1)
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        private static  List<string> GetCurrentAuthGroups(ISessionProvider sessionProvider)
        {
            var user = sessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            var auth_users = new List<String> { "*" };
            if (user != null)
            {
                auth_users.Add(user.user_id);
                auth_users.AddRange(user.groups);
            }
            return auth_users;
        }

        public static Bitmap GetImageBitmapFromFile(string filePath)
        {
            if(!File.Exists(filePath)) throw new FileNotFoundException(filePath);

            using (var imageBase = System.Drawing.Image.FromFile(filePath))
            {
                return new Bitmap(imageBase);
            }
        }
        public static void ProcessImage(JObject fileData, Bitmap image, RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone)
        {
            Size imageSize = new Size();
            ImageThumbnail it = new ImageThumbnail();
            fileData[ImageProcessor.IMAGE_S_BASE64] = it.CompressImage(image, 100, 75, out imageSize, 70, rotate);
            AddSize(fileData, imageSize, ImageProcessor.IMAGE_S_SIZE);

            fileData[ImageProcessor.IMAGE_M_BASE64] = it.CompressImage(image, 240, 180, out imageSize,90, rotate);
            AddSize(fileData, imageSize, ImageProcessor.IMAGE_M_SIZE);

            fileData[ImageProcessor.IMAGE_L_BASE64] = it.CompressImage(image, 1024, 768, out imageSize,90, rotate);
            AddSize(fileData, imageSize, ImageProcessor.IMAGE_L_SIZE);
            fileData[ImageProcessor.CHANGESET_NO] = 0;
        }
        private static void AddSize(JObject fileData, Size imageSize, string key)
        {
            fileData[key] = new JObject();
            fileData[key][ImageProcessor.WIDTH] = imageSize.Width;
            fileData[key][ImageProcessor.HEIGHT] = imageSize.Height;
        }



        public static JArray GetImageData(IDBService dbProxy, ISessionProvider sessionProvider, string fileHash, List<string> extraFields = null)
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

            var data = dbProxy.Get(ImageProcessor.MYPHOTO_COLLECTION, filter.ToString(), fields);
            if (data.Count == 0) throw new KeyNotFoundException(filter.ToString());
            var user = sessionProvider.GetValue<UserModel>(CommonConst.CommonValue.SESSION_USER_KEY);
            AddViewsToImageDetails(dbProxy,filter, data);
            AddLikesToImageDetails(dbProxy,filter, data, user);
            AddBookmarkToImageDetails(dbProxy,data, fileHash, user);

            return data;
        }

        private static void AddBookmarkToImageDetails(IDBService dbProxy, JArray data, string fileHash, UserModel user)
        {

            data.First()[ImageProcessor.IS_BOOKMARKED] = false;
            if (user != null)
            {
                var filter = new JObject();
                filter[CommonConst.CommonField.USER_ID] = user.user_id;
                var bookmark = dbProxy.FirstOrDefault(ImageProcessor.MYPHOTO_IMAGE_BOOKMARK_COLLECTION, filter.ToString());

                if (bookmark != null)
                {
                    if ((bookmark[ImageProcessor.IMAGES] as JArray).FirstOrDefault(f => f[ImageProcessor.FILE_HASH].ToString() == fileHash) != null)
                    {
                        data.First()[ImageProcessor.IS_BOOKMARKED] = true;
                    }
                }
            }
        }
        private static void AddLikesToImageDetails(IDBService dbProxy, JObject filter, JArray data, UserModel user)
        {
            var viewLike = dbProxy.Get(ImageProcessor.MYPHOTO_IMAGE_LIKE_COLLECTION, filter.ToString(), new List<string>() { ImageProcessor.COUNT, ImageProcessor.USERS });
            data[0][ImageProcessor.LIKES_COUNT] = 0;
            data[0][ImageProcessor.IS_LIKED] = false;
            if (viewLike.Count != 0)
            {
                data[0][ImageProcessor.LIKES_COUNT] = viewLike[0][ImageProcessor.COUNT];
                data[0][ImageProcessor.IS_LIKED] = false;

                if (user != null)
                {
                    if ((viewLike[0][ImageProcessor.USERS] as JArray).FirstOrDefault(f => f.ToString() == user.user_id) != null)
                    {
                        data[0][ImageProcessor.IS_LIKED] = true;
                    }
                }
            }
        }

        private static void AddViewsToImageDetails(IDBService dbProxy, JObject filter, JArray data)
        {
            var viewData = dbProxy.Get(ImageProcessor.MYPHOTO_IMAGE_VIEW_COLLECTION, filter.ToString(), new List<string>() { ImageProcessor.COUNT });
            data[0][ImageProcessor.VIEWS_COUNT] = 0;
            if (viewData.Count != 0)
            {
                data[0][ImageProcessor.VIEWS_COUNT] = viewData[0][ImageProcessor.COUNT];
            }
        }

    }
}
