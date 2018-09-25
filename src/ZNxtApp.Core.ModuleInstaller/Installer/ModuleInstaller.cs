using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.ModuleInstaller.Installer
{
    public class ModuleInstaller : InstallerBase, IModuleInstaller
    {
        private const string MODULE_INFO_FILE = "module" + CommonConst.CONFIG_FILE_EXTENSION;

        public ModuleInstaller(ILogger logger, IDBService dbProxy)
            : base(logger, dbProxy)
        {
        }

        public bool Install(string moduleFullName, IHttpContextProxy httpProxy, bool IsOverride = true)
        {
            try
            {
                _httpProxy = httpProxy;
                string moduleName = GetModuleName(moduleFullName);
                string moduleVerion = GetModuleVersion(moduleFullName);
                string moduleFolder = moduleFullName.Replace("/", "_");
                var moduleDir = string.Format("{0}\\{1}", ApplicationConfig.AppModulePath, moduleFolder);
                if (!Directory.Exists(moduleDir))
                {
                    DownloadNugetPackage(moduleFullName);
                }
                if (Directory.Exists(moduleDir))
                {
                    JObject moduleObject = new JObject();
                    moduleObject[CommonConst.CommonField.NAME] = GetModuleName(moduleFullName);
                    if (!CheckOverrideModule(moduleName, IsOverride, ref moduleObject) && !IsOverride)
                    {
                        return false;
                    }

                    UpdateModuleInfo(moduleDir, moduleName, moduleVerion, ref moduleObject);
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

                    _dbProxy.Update(CommonConst.Collection.MODULES, "{" + CommonConst.CommonField.DATA_KEY + " :'" + moduleName + "'}", moduleObject, true);
                    return true;
                }
                else
                {
                    _logger.Error(string.Format("Module directory not found {0}", moduleDir), null);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error installing module {0}, {1}", moduleFullName, ex.Message), ex);
                return false;
            }
        }

        private void DownloadNugetPackage(string moduleName)
        {
            var folderPath = string.Format("{0}\\{1}", ApplicationConfig.AppModulePath, moduleName.Replace("/", "_"));
            var filePath = string.Format("{0}.zip", folderPath);

            try
            {
                if (!File.Exists(filePath))
                {
                    WebClient client = new WebClientWithTimeout();
                    var downloadUrl = string.Format("{0}{1}", "https://www.nuget.org/api/v2/package/", moduleName);
                    client.DownloadFile(downloadUrl, filePath);
                }
                ZipFile.ExtractToDirectory(filePath, folderPath);
            }
            catch (Exception ex)
            {
                try
                {
                    File.Delete(filePath);
                    Directory.Delete(folderPath, true);
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message, e);
                }
                _logger.Error(string.Format("Error in downloading nuger package {0}, Error: {1}", moduleName, ex.Message), ex);
            }
        }

        private JObject CreateCollectionEntry(string name, string accessType)
        {
            JObject result = new JObject();
            result[CommonConst.CommonField.NAME] = name;
            result[CommonConst.CommonField.ACCESS_TYPE] = accessType;
            return result;
        }

        private void UpdateModuleInfo(string baseModulePath, string moduleName, string moduleVersion, ref JObject moduleObject)
        {
            string filePath = string.Format("{0}\\Content\\{1}", baseModulePath, MODULE_INFO_FILE);
            if (File.Exists(filePath))
            {
                var moduleFileData = JObjectHelper.GetJObjectFromFile(filePath);
                moduleFileData[CommonConst.CommonField.NAME] = moduleName;
                moduleFileData[CommonConst.CommonField.VERSION] = moduleVersion;
                moduleObject = JObjectHelper.Marge(moduleObject, moduleFileData, MergeArrayHandling.Union);
            }
            string fileNugetPackageInfo = string.Format("{0}\\{1}.nuspec", baseModulePath, moduleName);
            if (File.Exists(fileNugetPackageInfo))
            {
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(File.ReadAllText(fileNugetPackageInfo));
                var packageData = JObjectHelper.Serialize<XmlDocument>(xdoc);
                moduleObject[CommonConst.CommonField.PACKAGE_CONFIG] = packageData["package"]["metadata"];
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
            var collectionsPath = string.Format("{0}\\Content\\{1}", moduleDir, CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER);
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
                        _logger.Debug(string.Format("InstallCollections File:{0}", fi.FullName));

                        foreach (JObject joData in JObjectHelper.GetJArrayFromFile(fi.FullName))
                        {
                            joData[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
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
            var dllPath = string.Format("{0}\\lib\\net452\\", moduleDir);
            if (Directory.Exists(dllPath))
            {
                DirectoryInfo di = new DirectoryInfo(dllPath);
                FileInfo[] files = di.GetFiles("*.dll");
                CleanDBCollection(moduleName, CommonConst.Collection.DLLS);
                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item.FullName);
                    var contentType = _httpProxy.GetContentType(fi.FullName);
                    var joData = JObjectHelper.GetJObjectDbDataFromFile(fi, contentType, string.Format("{0}", di.FullName), moduleName);
                    WriteToDB(joData, moduleName, CommonConst.Collection.DLLS, CommonConst.CommonField.FILE_PATH);
                }
            }
        }

        private void InstallWWWRoot(string path, string moduleName)
        {
            var wwwrootPath = string.Format("{0}\\Content\\{1}", path, CommonConst.MODULE_INSTALL_WWWROOT_FOLDER);
            if (Directory.Exists(wwwrootPath))
            {
                DirectoryInfo di = new DirectoryInfo(wwwrootPath);
                FileInfo[] files = di.GetFiles("*.*", SearchOption.AllDirectories);

                CleanDBCollection(moduleName, CommonConst.Collection.STATIC_CONTECT);

                foreach (var item in files)
                {
                    FileInfo fi = new FileInfo(item.FullName);
                    var contentType = _httpProxy.GetContentType(fi);
                    var joData = JObjectHelper.GetJObjectDbDataFromFile(fi, contentType, di.FullName, moduleName);
                    WriteToDB(joData, moduleName, CommonConst.Collection.STATIC_CONTECT, CommonConst.CommonField.FILE_PATH);
                }
            }
        }

        private void WriteToDB(JObject joData, string moduleName, string collection, string compareKey)
        {
            OverrideData(joData, moduleName, compareKey, collection);
            if (!_dbProxy.Write(collection, joData))
            {
                _logger.Error(string.Format("Error while uploading file data {0}", joData.ToString()), null);
            }
        }

        private void OverrideData(JObject joData, string moduleName, string compareKey, string collection)
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
            _dbProxy.Write(collection, updateObject, updateOverrideFilter);
        }

        public JObject GetDetails(string moduleName)
        {
            JObject filter = new JObject();
            filter[CommonConst.CommonField.NAME] = GetModuleName(moduleName);
            return GetModule(filter);
        }

        public class WebClientWithTimeout : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = ((60 * 1000) * 60); // timeout in milliseconds (ms)
                return wr;
            }
        }
    }
}