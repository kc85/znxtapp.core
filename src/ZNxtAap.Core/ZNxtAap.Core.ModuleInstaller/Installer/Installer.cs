using Newtonsoft.Json.Linq;
using System;
using System.IO;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.ModuleInstaller.Installer
{
    public class Installer : InstallerBase, IModuleInstaller
    {
       
        public Installer(ILogger logger, IDBService dbProxy): base(logger,dbProxy)
        {
        }
        public bool Install(string moduleName,IHttpContextProxy httpProxy)
        {
            _httpProxy = httpProxy;
            var moduleDir =  string.Format("{0}\\{1}", ApplicationConfig.AppModulePath, moduleName);
            if (Directory.Exists(moduleDir))
            {
                InstallWWWRoot(moduleDir,moduleName);
                InstallDlls(moduleDir, moduleName);
                InstallCollections(moduleDir, moduleName);
                return true;
            }
            else
            {
                _logger.Error(string.Format("Module directory not found {0}", moduleDir),null);
                return false;
            }
        }

        private void InstallCollections(string moduleDir, string moduleName)
        {
            var collectionsPath = string.Format("{0}\\{1}", moduleDir, CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER);
            if (Directory.Exists(collectionsPath))
            {
                DirectoryInfo di = new DirectoryInfo(collectionsPath);
                FileInfo[] files = di.GetFiles(string.Format("*{0}", CommonConst.CONFIG_FILE_EXTENSION));
                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item.FullName);
                    var collectionName = fi.Name.Replace(fi.Extension, "");
                    if (collectionName.Contains("."))
                    {
                        continue;
                    }
                    CleanDBCollection(moduleName, collectionName);

                    foreach (JObject joData in JObjectHelper.GetJArrayFromFile(fi.FullName))
                    {
                        joData[CommonConst.CommonField.DISPLAY_ID] = Guid.NewGuid().ToString(); 
                        joData[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = DateTime.Now;
                        joData[CommonConst.CommonField.MODULE_NAME] = moduleName;
                        joData[CommonConst.CommonField.ÌS_OVERRIDE] = false;
                        joData[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;
                        WriteToDB(joData, moduleName, collectionName, CommonConst.CommonField.DATA_KEY);
                    }
                }
            }
        }

        private void InstallDlls(string moduleDir, string moduleName)
        {
            var dllPath = string.Format("{0}\\{1}", moduleDir, CommonConst.MODULE_INSTALL_DLLS_FOLDER);
            if (Directory.Exists(dllPath))
            {
                DirectoryInfo di = new DirectoryInfo(dllPath);
                FileInfo[] files = di.GetFiles("*.dll");
                CleanDBCollection(moduleName, CommonConst.Collection.DLLS);
                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item.FullName);
                    var contentType = _httpProxy.GetContentType(fi.FullName);
                    var joData = GetJObjectData(fi, contentType, string.Format("{0}\\",di.FullName), moduleName);
                    WriteToDB(joData, moduleName, CommonConst.Collection.DLLS, CommonConst.CommonField.FILE_PATH);
                }
            }
            
        }

        private void InstallWWWRoot(string path, string moduleName)
        {
            var wwwrootPath = string.Format("{0}\\{1}", path, CommonConst.MODULE_INSTALL_WWWROOT_FOLDER);
            if (Directory.Exists(wwwrootPath))
            {
                DirectoryInfo di = new DirectoryInfo(wwwrootPath);
                FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

                CleanDBCollection(moduleName, CommonConst.Collection.STATIC_CONTECT);

                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item.FullName);
                    var contentType = _httpProxy.GetContentType(fi.FullName);
                    var joData = GetJObjectData(fi, contentType, di.FullName, moduleName);
                    WriteToDB(joData, moduleName, CommonConst.Collection.STATIC_CONTECT, CommonConst.CommonField.FILE_PATH);
                }
            }
        }

        private void CleanDBCollection(string moduleName,string collection)
        {
            string cleanupFilter = "{ " + CommonConst.CommonField.MODULE_NAME + ":'" + moduleName + "'}";
            _dbProxy.Collection = collection;
            _dbProxy.Delete(cleanupFilter);
        }

        private void WriteToDB(JObject joData, string moduleName, string collection, string compareKey)
        {
            OverrideData(joData, moduleName, compareKey);
            if (!_dbProxy.WriteData(joData))
            {
                _logger.Error(string.Format("Error while uploading file data {0}", joData.ToString()), null);
            }
        }

        private void OverrideData(JObject joData, string moduleName, string compareKey)
        {
            string updateOverrideFilter = "{ $and: [ { is_override:false }, {" + compareKey + ":'" + joData[compareKey].ToString() + "'}] } ";
            var updateObject = new JObject();
            updateObject[CommonConst.CommonField.ÌS_OVERRIDE] = true;
            JArray lastOverrides = new JArray();
            if (updateObject[CommonConst.CommonField.LAST_OVERRIDES] != null)
            {
                lastOverrides = updateObject[CommonConst.CommonField.LAST_OVERRIDES] as JArray;
            }
            lastOverrides.Add(updateObject[CommonConst.CommonField.OVERRIDE_BY]);
            updateObject[CommonConst.CommonField.LAST_OVERRIDES] = lastOverrides;

            updateObject[CommonConst.CommonField.OVERRIDE_BY] = moduleName;
            _dbProxy.Update(updateOverrideFilter, updateObject);
        }

        private JObject GetJObjectData(FileInfo fi, string contentType, string basepath, string moduleName)
        {
            string fileData = GetData(fi.FullName, contentType);
            string wwwwpath = fi.FullName.Replace(basepath, "").Replace("\\", "/");
            JObject data = new JObject();
            data[CommonConst.CommonField.DISPLAY_ID] = Guid.NewGuid().ToString();
            data[CommonConst.CommonField.FILE_PATH] = wwwwpath;
            data[CommonConst.CommonField.DATA] = fileData;
            data[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = DateTime.Now;
            data[CommonConst.CommonField.FILE_SIZE] = fi.Length;
            data[CommonConst.CommonField.MODULE_NAME] = moduleName;
            data[CommonConst.CommonField.CONTENT_TYPE] = contentType;
            data[CommonConst.CommonField.ÌS_OVERRIDE] = false;
            data[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;
            return data;
        }

        private string GetData(string path, string contentType)
        {
            if (CommonUtility.IsTextConent(contentType))
            {
                return GetTextData(path);
            }
            else
            {
                return GetBinaryData(path);

            }
        }

        private string GetBinaryData(string path)
        {
            byte[] fileData = File.ReadAllBytes(path);
            return CommonUtility.GetBase64(fileData);
        }

        private string GetTextData(string path)
        {
            return File.ReadAllText(path);
        }
    }
}