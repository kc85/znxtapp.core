using System;
using System.Collections.Generic;
using System.IO;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class KeyValueFileStorage : IKeyValueStorage
    {
        private IEncryption _encryption;
        private IAppSettingService _appSettingService;
        private string _storageBasePath = string.Empty;

        public KeyValueFileStorage(IEncryption encryption, IAppSettingService appSettingService)
        {
            _encryption = encryption;
            _appSettingService = appSettingService;
            _storageBasePath = appSettingService.GetAppSettingData("KeyValueFileStoragePath");
        }

        public bool Delete(string bucket, string key)
        {
            var path = GetPath(bucket, key);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            else
            {
                return false;
            }
        }

        public T Get<T>(string bucket, string key, string encriptionKey = null)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBuckets()
        {
            throw new NotImplementedException();
        }

        public List<string> GetKeys(string bucket)
        {
            throw new NotImplementedException();
        }

        public bool Put<T>(string bucket, string key, T data, string encriptionKey = null)
        {
            if (typeof(T) == typeof(Byte[]))
            {
                byte[] byteData = data as Byte[];
                if (!string.IsNullOrEmpty(encriptionKey))
                {
                    byteData = _encryption.Encrypt(byteData, encriptionKey);
                }
                File.WriteAllBytes(GetPath(bucket, key), byteData);
                return true;
            }
            else
            {
                throw new NotSupportedException(string.Format("Type not supported {0}", typeof(T).ToString()));
            }
        }

        private string GetPath(string bucket, string key = null)
        {
            string path = string.Format("{0}\\{1}", _storageBasePath, bucket);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!string.IsNullOrEmpty(key))
            {
                path = string.Format("{0}\\{1}.zdata", path, key);
            }
            return path;
        }

        public byte[] Get(string bucket, string key, string encriptionKey = null)
        {
            var path = GetPath(bucket, key);
            if (!File.Exists(path))
            {
                throw new KeyNotFoundException(key);
            }

            byte[] byteData = File.ReadAllBytes(path);
            if (!string.IsNullOrEmpty(encriptionKey))
            {
                byteData = _encryption.Decrypt(byteData, encriptionKey);
            }
            return byteData;
        }
    }
}