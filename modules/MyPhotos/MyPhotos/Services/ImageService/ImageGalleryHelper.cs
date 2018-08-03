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
        public static string RotateImage(string base64, int quality)
        {
            string base64Data = "";
            using (Image img = GetImage(base64))
            {
                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.

                EncoderParameters encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.

                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);

                encoderParameters.Param[0] = encoderParameter;

                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid), encoderParameters);
                    base64Data = Convert.ToBase64String(ms.ToArray());
                }

            }
            return base64Data;
        }

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
    }
}
