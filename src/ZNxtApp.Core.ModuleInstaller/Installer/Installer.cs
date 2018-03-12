using Newtonsoft.Json.Linq;
using System;
using System.IO;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;
using System.Linq;

namespace ZNxtApp.Core.ModuleInstaller.Installer
{
    public class Installer : InstallerBase, IModuleInstaller
    {
        private const string MODULE_INFO_FILE = "module" + CommonConst.CONFIG_FILE_EXTENSION;

        public Installer(ILogger logger, IDBService dbProxy)
            : base(logger, dbProxy)
        {
        }

        public bool Install(string moduleName, IHttpContextProxy httpProxy, bool IsOverride = true)
        {
            _httpProxy = httpProxy;
            var moduleDir = string.Format("{0}\\{1}", ApplicationConfig.AppModulePath, moduleName);
            if (Directory.Exists(moduleDir))
            {
                JObject moduleObject = new JObject();
                moduleObject[CommonConst.CommonField.DATA_KEY] = moduleName;
                if (!CheckOverrideModule(moduleName, IsOverride, ref moduleObject) && !IsOverride)
                {
                    return false;
                }
                UpdateModuleInfo(moduleDir, ref moduleObject);
                var moduleCollections = new JArray();
                if (moduleObject[CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER] != null)
                {
                    moduleCollections = moduleObject[CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER] as JArray;
                }
                else
                {
                    moduleObject[CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER] = moduleCollections;
                }                
                moduleCollections.Add(CreateCollectionEntry(CommonConst.Collection.STATIC_CONTECT, CommonConst.CollectionAccessTypes.READONLY));
                moduleCollections.Add(CreateCollectionEntry(CommonConst.Collection.DLLS, CommonConst.CollectionAccessTypes.READONLY));

                InstallWWWRoot(moduleDir, moduleName);
                InstallDlls(moduleDir, moduleName);
                InstallCollections(moduleDir, moduleName, moduleCollections);
                _dbProxy.Collection = CommonConst.Collection.MODULES;
                _dbProxy.Update("{" + CommonConst.CommonField.DATA_KEY + " :'" + moduleName + "'}", moduleObject, true);
                return true;
            }
            else
            {
                _logger.Error(string.Format("Module directory not found {0}", moduleDir), null);
                return false;
            }
        }

        private JObject CreateCollectionEntry(string name, string accessType)
        {
            JObject result = new JObject();
            result[CommonConst.CommonField.NAME] = name;
            result[CommonConst.CommonField.ACCESS_TYPE] = accessType;
            return result;
        }

        private void UpdateModuleInfo(string baseModulePath, ref JObject moduleObject)
        {
            string filePath = string.Format("{0}\\{1}", baseModulePath, MODULE_INFO_FILE);
            if (File.Exists(filePath))
            {
                var moduleFileData = JObjectHelper.GetJObjectFromFile(filePath);
                moduleObject = JObjectHelper.Marge(moduleObject, moduleFileData, MergeArrayHandling.Union);
            }
        }

        private bool CheckOverrideModule(string moduleName, bool IsOverride, ref JObject moduleObject)
        {
            var data = GetModule(moduleObject);
            if (!IsOverride && data != null)
            {
                _logger.Error(string.Format("Module already installed : {0}", moduleName), null);
                return false;
            }
            if (data != null)
            {
                moduleObject = JObjectHelper.Marge(data, moduleObject, MergeArrayHandling.Union);
            }

            return true;
        }

        private void InstallCollections(string moduleDir, string moduleName, JArray moduleCollections)
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
                    //Find collection in config

                    var collectionConfig = moduleCollections.FirstOrDefault(f => f[CommonConst.CommonField.NAME].ToString() == collectionName);
                    if (collectionConfig != null)
                    {
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
                    var joData = GetJObjectData(fi, contentType, string.Format("{0}\\", di.FullName), moduleName);
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
                    var contentType = _httpProxy.GetContentType(fi);
                    var joData = GetJObjectData(fi, contentType, di.FullName, moduleName);
                    WriteToDB(joData, moduleName, CommonConst.Collection.STATIC_CONTECT, CommonConst.CommonField.FILE_PATH);
                }
            }
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
            string updateOverrideFilter = "{ $and: [ { " + CommonConst.CommonField.IS_OVERRIDE + ":false }, {" + compareKey + ":'" + joData[compareKey].ToString() + "'}] } ";
            var updateObject = new JObject();
            updateObject[CommonConst.CommonField.ÌS_OVERRIDE] = true;
            //JArray lastOverrides = new JArray();
            //if (updateObject[CommonConst.CommonField.LAST_OVERRIDES] != null)
            //{
            //    lastOverrides = updateObject[CommonConst.CommonField.LAST_OVERRIDES] as JArray;
            //}
            //lastOverrides.Add(updateObject[CommonConst.CommonField.OVERRIDE_BY]);
            //updateObject[CommonConst.CommonField.LAST_OVERRIDES] = lastOverrides;

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

        public JObject GetDetails(string moduleName)
        {
            var moduleDir = string.Format("{0}\\{1}", ApplicationConfig.AppModulePath, moduleName);
            if (Directory.Exists(moduleDir))
            {
                string filePath = string.Format("{0}\\{1}", moduleDir, MODULE_INFO_FILE);
                if (File.Exists(filePath))
                {
                    var moduleFileData = JObjectHelper.GetJObjectFromFile(filePath);
                    return moduleFileData;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }
    }
}