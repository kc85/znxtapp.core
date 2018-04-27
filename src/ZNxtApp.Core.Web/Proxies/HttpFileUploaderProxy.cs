using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Helpers;
using System.IO;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Web.Proxies
{
    public partial class HttpContextProxy : IHttpFileUploader
    {
        public List<string> GetFiles()
        {
            List<string> files = new List<string>();
            for (int i = 0; i < _context.Request.Files.Count; i++)
            {
                files.Add(_context.Request.Files[i].FileName);
            }
            return files;
        }

        public string Save(string fileName, string destination = null)
        {
            if (destination == null)
            {
                destination = string.Format("{0}\\{1}", ApplicationConfig.AppTempFolderPath, fileName);

            }
            for (int i = 0; i < _context.Request.Files.Count; i++)
            {
                if (_context.Request.Files[i].FileName == fileName)
                {
                    _context.Request.Files[i].SaveAs(destination);
                    return destination;
                }
            }
            return string.Empty;
        }

        public JObject SaveToDB(IDBService dbProxy, string fileName, string baseFolder, string collection, string updateFilter = null)
        {
            string destination = string.Format("{0}\\{1}{2}", ApplicationConfig.AppTempFolderPath, CommonUtility.RandomString(5), fileName);
            if (!string.IsNullOrEmpty(Save(fileName, destination)))
            {

                FileInfo fi = new FileInfo(destination);
                var contentType = GetContentType(fi);
                var fileUploadData = JObjectHelper.GetJObjectDbDataFromFile(fi, contentType, ApplicationConfig.AppTempFolderPath, "ZNxtAppUpload", baseFolder);
                File.Delete(destination);
                if (string.IsNullOrEmpty(updateFilter))
                {
                    dbProxy.Write(collection, fileUploadData);
                    return dbProxy.FirstOrDefault(collection, CommonConst.CommonField.DISPLAY_ID, fileUploadData[CommonConst.CommonField.DISPLAY_ID].ToString());
                }
                else
                {
                    fileUploadData.Remove(CommonConst.CommonField.DISPLAY_ID);
                    dbProxy.Write(collection, fileUploadData, updateFilter, true, MergeArrayHandling.Replace);
                    return dbProxy.FirstOrDefault(collection, updateFilter);
                }
            }

            return new JObject();
        }
    }
}
