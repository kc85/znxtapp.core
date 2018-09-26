using System;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IKeyValueStorage
    {
        /// <summary>
        /// Get the value from Key Value store.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="encriptionKey"></param>
        /// <returns></returns>
        T Get<T>(string bucket, string key, string encriptionKey = null);

        byte[] Get(string bucket, string key, string encriptionKey = null);

        /// <summary>
        /// Put value to bucket
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="encriptionKey"></param>
        /// <returns></returns>
        bool Put<T>(string bucket, string key, T data, string encriptionKey = null);

        /// <summary>
        /// Delete value from bucket
        /// </summary>
        /// <param name="bucket"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Delete(string bucket, string key);

        /// <summary>
        /// Get all keys from bucket
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        List<String> GetKeys(string bucket);

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        List<String> GetBuckets();


        /// <summary>
        /// Delete Bucket
        /// </summary>
        /// <returns></returns>
        bool DeleteBucket(string bucket);
    }
}