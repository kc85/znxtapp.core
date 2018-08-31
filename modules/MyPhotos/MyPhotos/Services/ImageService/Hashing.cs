using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyPhotos.Services.ImageService
{
    public static class Hashing
    {
        public static string GetFileHash(string fileName)
        {
           // return string.Empty;
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate,
              FileAccess.Read))
            {
                using (var bufferedStream = new BufferedStream(fileStream, 1024 * 32))
                {
                    var sha = new SHA256Managed();
                    byte[] checksum = sha.ComputeHash(bufferedStream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
        }
        public static string GetFileHash(byte[] byteArr)
        {
            // return string.Empty;

            var sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(byteArr);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);


        }
    }
}
