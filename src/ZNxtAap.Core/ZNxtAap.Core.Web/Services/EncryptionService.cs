using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class EncryptionService : IEncryption
    {

        private string _encryptionKey = "f3856ad364e35";
        private string _hashKey = "2932lkn23asdad";

        public EncryptionService()
        {
            if (ConfigurationManager.AppSettings["EncryptionKey"] != null)
            {
                _encryptionKey = ConfigurationManager.AppSettings["EncryptionKey"];
            }
            if (ConfigurationManager.AppSettings["HashKey"] != null)
            {
                _hashKey = ConfigurationManager.AppSettings["HashKey"];
            }
        }
        public string GetHash(string inputString)
        {
            return GetHash(inputString, _hashKey);
        }

        public string GetHash(string inputString, string encryptionKey)
        {
            return Encrypt(inputString, encryptionKey);
        }

        public string Encrypt(string inputString)
        {
            return Encrypt(inputString, _encryptionKey);
        }

        public string Encrypt(string inputString, string encryptionKey)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(inputString);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            byte[] trimmedBytes = new byte[24];
            var byteArr = UTF8Encoding.UTF8.GetBytes(encryptionKey);
            Buffer.BlockCopy(byteArr, 0, trimmedBytes, 0, 24);
            tripleDES.Key = trimmedBytes;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public string Decrypt(string inputString)
        {
            return Decrypt(inputString, _encryptionKey);

        }

        public string Decrypt(string inputString, string encryptionKey)
        {
            byte[] inputArray = Convert.FromBase64String(inputString);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            byte[] trimmedBytes = new byte[24];
            var byteArr = UTF8Encoding.UTF8.GetBytes(encryptionKey);
            Buffer.BlockCopy(byteArr, 0, trimmedBytes, 0, 24);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
