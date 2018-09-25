using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Util
{
    public class StaticContentHandler
    {
        public static byte[] GetContent(IDBService dbProxy, ILogger logger, string path)
        {
            string wwwrootpath = ApplicationConfig.AppWWWRootPath;

            JObject document = (JObject)dbProxy.Get(CommonConst.Collection.STATIC_CONTECT, GetFilter(path)).First;
            if (document != null)
            {
                var data = document[CommonConst.CommonField.DATA];
                if (data != null)
                {
                    if (CommonUtility.IsTextConent(document[CommonConst.CommonField.CONTENT_TYPE].ToString()))
                    {
                        return Encoding.ASCII.GetBytes(data.ToString());
                    }
                    else
                    {
                        byte[] dataByte = System.Convert.FromBase64String(data.ToString());
                        return dataByte;
                    }
                }
            }
            else
            {
                string filePath = string.Format("{0}{1}", wwwrootpath, path);
                if (File.Exists(filePath))
                {
                    return File.ReadAllBytes(filePath);
                }
            }
            return null;
        }

        private static string GetFilter(string path)
        {
            path = path.Replace("\\", "/");
            return "{ $and: [ { " + CommonConst.CommonField.IS_OVERRIDE + ":{ $ne: true}  }, {'" + CommonConst.CommonField.FILE_PATH + "':  {$regex :'^" + path.ToLower() + "$','$options' : 'i'}}] }";
        }

        public static string GetStringContent(IDBService dbProxy, ILogger _logger, string path)
        {
            string wwwrootpath = ApplicationConfig.AppWWWRootPath;

            JObject document = (JObject)dbProxy.Get(CommonConst.Collection.STATIC_CONTECT, GetFilter(path)).First;
            if (document != null)
            {
                var data = document[CommonConst.CommonField.DATA];
                if (data != null)
                {
                    return data.ToString();
                }
            }
            else
            {
                string filePath = string.Format("{0}{1}", wwwrootpath, path);
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            return null;
        }

        public static string MappedUriPath(string url)
        {
            if (url.IndexOf(ApplicationConfig.AppBackendPath) == 0)
            {
                var path = url.Remove(0, ApplicationConfig.AppBackendPath.Length);
                path = string.Format("/{0}{1}", CommonConst.CommonValue.APP_BACKEND_FOLDERPATH, path);
                return path;
            }
            else
            {
                var path = string.Format("/{0}{1}", CommonConst.CommonValue.APP_FRONTEND_FOLDERPATH, url);

                return path;
            }
        }

        public static string UnmappedUriPath(string path)
        {
            if (path.IndexOf(string.Format("/{0}", CommonConst.CommonValue.APP_BACKEND_FOLDERPATH)) == 0)
            {
                path = path.Remove(0, CommonConst.CommonValue.APP_BACKEND_FOLDERPATH.Length + 1);
                path = string.Format("{0}{1}", ApplicationConfig.AppBackendPath, path);
                return path;
            }
            else
            {
                path = path.Remove(0, CommonConst.CommonValue.APP_FRONTEND_FOLDERPATH.Length + 1);
                return path;
            }
        }

        public static bool IsAdminPage(string url)
        {
            return url.IndexOf(string.Format("/{0}", CommonConst.CommonValue.APP_BACKEND_FOLDERPATH)) == 0;
        }
    }
}