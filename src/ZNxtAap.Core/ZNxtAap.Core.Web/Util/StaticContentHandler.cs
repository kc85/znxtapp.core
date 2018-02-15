using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Util
{
    public class StaticContentHandler
    {

        public static byte[] GetContent(IDBService dbProxy, ILogger logger, string path)
        {
            string wwwrootpath = ApplicationConfig.AppWWWRootPath;

            dbProxy.Collection = CommonConst.Collection.STATIC_CONTECT;
            JObject document = (JObject)dbProxy.Get(GetFilter(path)).First;
            if (document != null)
            {
                var data = document[CommonConst.CommonField.DATA];
                if (data != null)
                {
                    if (document[CommonConst.CommonField.CONTENT_TYPE].ToString().Contains("text"))
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
            return "{ $and: [ { " + CommonConst.CommonField.IS_OVERRIDE + ":{ $ne: true}  }, {'" + CommonConst.CommonField.FILE_PATH + "':  {$regex :'^" + path.ToLower() + "$','$options' : 'i'}}] }";
        }
    }
}