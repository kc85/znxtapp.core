using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotos.Services.ImageService
{
    public class ImageThumbnail
    {
    

        public string CompressImage(Bitmap image, int maxWidth, int maxHeight, int quality = 90)
        {
            //return string.Empty;

            // Get the image's original width and height

            int originalWidth = image.Width;

            int originalHeight = image.Height;

            // To preserve the aspect ratio

            float ratioX = (float)maxWidth / (float)originalWidth;

            float ratioY = (float)maxHeight / (float)originalHeight;

            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio

            int newWidth = (int)(originalWidth * ratio);

            int newHeight = (int)(originalHeight * ratio);

            // Convert other formats (including CMYK) to RGB.

            using (Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb))
            {

                // Draws the image in the specified size with quality mode set to HighQuality

                using (Graphics graphics = Graphics.FromImage(newImage))
                {

                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                }

                // Get an ImageCodecInfo object that represents the JPEG codec.

                ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);

                // Create an Encoder object for the Quality parameter.

                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.

                EncoderParameters encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.

                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);

                encoderParameters.Param[0] = encoderParameter;

                using (var ms = new MemoryStream())
                {
                    newImage.Save(ms, imageCodecInfo, encoderParameters);
                    var imageBase64 = Convert.ToBase64String(ms.ToArray());
                    // newImage.Save(@"C:\temp\t\" + RandomString(10) + ".png", imageCodecInfo, encoderParameters);
                    return imageBase64;
                }
            }
        }
        public string RandomString(int size, bool lowerCase=true)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }  

        /// <summary>

        /// Method to get encoder infor for given image format.

        /// </summary>

        /// <param name="format">Image format</param>

        /// <returns>image codec info.</returns>

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {

            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);

        }
    }
}
