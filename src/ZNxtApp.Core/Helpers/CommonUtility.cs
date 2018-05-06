﻿using System;
using System.Linq;
using System.Text;

namespace ZNxtApp.Core.Helpers
{
    public static class CommonUtility
    {
        public static string GetNewID()
        {
            return GetUnixTimestamp(DateTime.Now) + RandomString(3) + RandomNumber(5);
        }
        public static Int32 GetUnixTimestamp(DateTime dt)
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            TimeSpan unixTicks = new TimeSpan(dt.Ticks) - epochTicks;
            return (Int32)unixTicks.TotalSeconds;
        }

        public static string GetBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        public static string GetBase64(string data)
        {
            return GetBase64(Encoding.ASCII.GetBytes(data));
        }

        public static string GenerateTxnId(string prefix = "HT")
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);

            TimeSpan unixTicks = new TimeSpan(DateTime.Now.Ticks) - epochTicks;
            string txnId = prefix + RandomNumber(2) + unixTicks.Ticks.ToString();

            return txnId;
        }

        private static Random random = new Random();
        private static object syncObj = new object();
        public static string RandomString(int length)
        {
            lock (syncObj)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }

        public static string RandomNumber(int length)
        {
            lock (syncObj)
            {
                const string chars = "0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }
        }

        

        public static string GetAppConfigValue(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }

        public static bool IsTextConent(string contentType)
        {
            return contentType.Contains("text/") || contentType.Contains("application/json") || contentType.Contains("application/xml");
        }
        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        
    }
}