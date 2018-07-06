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
        public byte[] GetFileData(string fileName)
        {
            for (int i = 0; i < _context.Request.Files.Count; i++)
            {
                if (_context.Request.Files[i].FileName == fileName)
                {
                    BinaryReader b = new BinaryReader(_context.Request.Files[i].InputStream);
                    byte[] binData = b.ReadBytes(_context.Request.Files[i].ContentLength);
                    return binData;
                }
            }
            return null;
        }

        public string Save(string fileName, string destination = null, string fileBase64Data = null)
        {
            if (destination == null)
            {
                destination = string.Format("{0}\\{1}", ApplicationConfig.AppTempFolderPath, fileName);

            }
            if (fileBase64Data != null)
            {
                _logger.Debug(string.Format("Saving file data from base 64.Name {0} Data length {1}",fileName, fileBase64Data.Length));

                byte[] decodedByteArray = Convert.FromBase64String(fileBase64Data);
                File.WriteAllBytes(destination, decodedByteArray);
                return destination;
            }
            else
            {
                for (int i = 0; i < _context.Request.Files.Count; i++)
                {
                    if (_context.Request.Files[i].FileName == fileName)
                    {
                        _logger.Debug(string.Format("Saving file data from Request.Name {0} Data length {1}", fileName,_context.Request.Files[i].ContentLength));
                        _context.Request.Files[i].SaveAs(destination);
                        return destination;
                    }
                }
            }
            return string.Empty;
        }

        public JObject SaveToDB(IDBService dbProxy, string fileName, string baseFolder, string collection, string updateFilter = null, string fileBase64Data = null)
        {
            string destination = string.Format("{0}\\{1}{2}", ApplicationConfig.AppTempFolderPath, CommonUtility.RandomString(5), fileName);
            if (!string.IsNullOrEmpty(Save(fileName, destination,fileBase64Data)))
            {

                FileInfo fi = new FileInfo(destination);
                var contentType = GetContentType(fi);
                var fileUploadData = JObjectHelper.GetJObjectDbDataFromFile(fi, contentType, ApplicationConfig.AppTempFolderPath, "ZNxtAppUpload", baseFolder);
                File.Delete(destination);
                if (string.IsNullOrEmpty(updateFilter))
                {
                    _logger.Debug(string.Format("SaveToDB Writing new file  Name {0} ", fileName));

                    dbProxy.Write(collection, fileUploadData);
                    return dbProxy.FirstOrDefault(collection, CommonConst.CommonField.DISPLAY_ID, fileUploadData[CommonConst.CommonField.DISPLAY_ID].ToString());
                }
                else
                {
                    _logger.Debug(string.Format("SaveToDB updating file  Name {0} ", fileName));
                    fileUploadData.Remove(CommonConst.CommonField.DISPLAY_ID);
                    dbProxy.Write(collection, fileUploadData, updateFilter, true, MergeArrayHandling.Replace);
                    return dbProxy.Get(collection, updateFilter,new List<string>() { CommonConst.CommonField.DISPLAY_ID, CommonConst.CommonField.FILE_PATH }).First as JObject;
                }
            }

            return null;
        }
    }
}
