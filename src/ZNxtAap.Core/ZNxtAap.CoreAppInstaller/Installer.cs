﻿using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Enums;
using ZNxtAap.Core.Helpers;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.CoreAppInstaller
{
    /// <summary>
    /// 
    /// </summary>
    public class Installer : IAppInstaller
    {

        private static object lockObject = new object();
        private AppInstallStatus _status;
        private IPingService _pingService;
        private DataBuilderHelper _dataBuilderHelper;
        private const string PREREQUISITE_STATUS_API = "/install/checkprerequisite";
        private const string CHECK_STATUS_API = "/install/checkstatus";
        private const string INSTALL_START_API = "/install/start";
        private static IAppInstaller _appInstaller;
        private static bool IsPrerequisiteCheck = false;
        private const string defaultResourceName = "index.html";
        private const string _installStatusFile = "install_status." + CommonConst.CONFIG_FILE_EXTENSION;
        private readonly ILogger _logger;
        private readonly IEncryption _encryptionService;
        private readonly IDBService _dbProxy;
        private readonly IModuleInstaller _moduleInstaller; 

        private Installer(IPingService pingService, DataBuilderHelper dataBuilderHelper, ILogger logger, IDBService dbProxy, IEncryption encryptionService, IModuleInstaller moduleInstaller)
        {
            _pingService = pingService;
            _dataBuilderHelper = dataBuilderHelper;
            _logger = logger;
            _dbProxy = dbProxy;
            _encryptionService = encryptionService;
            _moduleInstaller = moduleInstaller;
            GetInstallStatus();
        }

        public static IAppInstaller GetInstance(IPingService pingService, DataBuilderHelper dataBuilderHelper, ILogger logger, IDBService dbProxy, IEncryption encryptionService, IModuleInstaller moduleInstaller)
        {
            if (_appInstaller == null)
            {
                lock (lockObject)
                {
                    _appInstaller = new Installer(pingService, dataBuilderHelper, logger, dbProxy, encryptionService, moduleInstaller);
                }
            }
            return _appInstaller;
        }

        public AppInstallStatus Status
        {
            get { return _status; }
        }

        public void Install(IHttpContextProxy httpProxy)
        {
            RequestHandler(httpProxy);
        }

        private void RequestHandler(IHttpContextProxy httpProxy)
        {
            string requestResource = httpProxy.GetURIAbsolutePath();
            if (!HandleAPI(httpProxy, requestResource))
            {
                string resourceName = GetResourceName(ref requestResource);

                string resourceData = Resource.ResourceManager.GetString(resourceName);

                httpProxy.ContentType = httpProxy.GetMimeType(requestResource);
                if (resourceData != null)
                {
                    httpProxy.SetResponse(CommonConst._200_OK, resourceData);
                }
                else
                {
                    httpProxy.SetResponse(CommonConst._404_RESOURCE_NOT_FOUND);
                }
            }
        }

        private bool HandleAPI(IHttpContextProxy httpProxy, string requestResource)
        {
            switch (requestResource)
            {
                case CHECK_STATUS_API:
                    httpProxy.SetResponse(CommonConst._200_OK, GetStatus());
                    httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
                    return true;

                case PREREQUISITE_STATUS_API:
                    httpProxy.SetResponse(CommonConst._200_OK, CheckAccess());
                    httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
                    return true;

                case INSTALL_START_API:
                    StartInstall(httpProxy);
                    return true;
            }
            return false;
        }

        private JObject GetStatus()
        {
            var response = _dataBuilderHelper.GetResponseObject(CommonConst._200_OK);
            _dataBuilderHelper.AddData(response, "status", _status.ToString()).AddData(response, "is_prerequisite_check", IsPrerequisiteCheck);
            return response;
        }

        private void StartInstall(IHttpContextProxy httpProxy)
        {
            UpdateInstallStatus(AppInstallStatus.Start);
            var requestData = httpProxy.GetRequestBody<AppInstallerConfig>();
            if (requestData == null)
            {
                httpProxy.SetResponse(CommonConst._400_BAD_REQUEST);
                httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
                return;
            }
            UpdateInstallStatus(AppInstallStatus.Inprogress);

            WriteCustomConfig(requestData);

            RunInstallScripts();

            InstallModule(requestData, httpProxy);

            UpdateInstallStatus(AppInstallStatus.Finish);
            httpProxy.SetResponse(CommonConst._200_OK, GetStatus());
            httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
        }

        private void InstallModule(AppInstallerConfig requestData,IHttpContextProxy httpProxy)
        {
            foreach (var item in requestData.DefaultModules)
            {
                _moduleInstaller.Install(item, httpProxy);
            }
        }

        private void WriteCustomConfig(AppInstallerConfig requestData)
        {
            JObject customConfig = JObject.Parse(@"{
                    'groups': [
                        'user',
                        'sys_admin'
                    ]
                }");

            customConfig[CommonConst.CommonField.USER_TYPE] = "Email";
            customConfig[CommonConst.CommonField.IS_EMAIL_VALIDATE] = true;
            customConfig[CommonConst.CommonField.DATA_KEY] =
                customConfig[CommonConst.CommonField.NAME] =
                customConfig[CommonConst.CommonField.EMAIL] =
                customConfig[CommonConst.CommonField.USER_ID] = requestData.AdminAccount;
            customConfig[CommonConst.CommonField.PASSWORD] = _encryptionService.Encrypt(requestData.AdminPassword);

            JArray configData = new JArray();
            configData.Add(customConfig);

            var path = GetCustomConfigDirectoryPath();
            string configFile = string.Format("{0}\\{1}{2}", path, CommonConst.Collection.USERS, CommonConst.CONFIG_FILE_EXTENSION);
            JObjectHelper.WriteJSONData(configFile, configData);

            customConfig = new JObject();
            customConfig[CommonConst.CommonField.DATA_KEY] = CommonConst.CommonField.NAME;
            customConfig[CommonConst.CommonField.VALUE] = requestData.Name;
            configData = new JArray();
            configData.Add(customConfig);
            configFile = string.Format("{0}\\{1}{2}", path, CommonConst.Collection.APP_INFO, CommonConst.CONFIG_FILE_EXTENSION);
            JObjectHelper.WriteJSONData(configFile, configData);

            configData = new JArray();
            foreach (var item in requestData.DefaultModules)
            {
                customConfig = new JObject();
                customConfig[CommonConst.CommonField.DATA_KEY] = item;
                customConfig[CommonConst.CommonField.VALUE] = item;
                configData.Add(customConfig);
            }
            configFile = string.Format("{0}\\{1}{2}", path, CommonConst.Collection.DEFAULT_INSTALL_MODULES, CommonConst.CONFIG_FILE_EXTENSION);
            JObjectHelper.WriteJSONData(configFile, configData);
        }

        private string GetCustomConfigDirectoryPath()
        {
            var tempFolder = ApplicationConfig.AppTempFolder;
            string path = string.Format("{0}\\Collections", tempFolder);
            var di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
            }
            return path;
        }

        private void RunInstallScripts()
        {
            _logger.Debug("START RunInstallScripts");

            var serverpath = string.Format("{0}\\InstallScripts\\Collections", ApplicationConfig.AppBinPath);
            var environment = CommonUtility.GetAppConfigValue(CommonConst.ENVIRONMENT_SETTING_KEY);
            environment = environment == null ? string.Empty : environment;
            string envExtension = string.Format(".{0}{1}", environment, CommonConst.CONFIG_FILE_EXTENSION);
            WriteConfigFileToDB(serverpath);

            /// Install Env specific files.
            if (!string.IsNullOrEmpty(environment))
            {
                string[] envFiles = Directory.GetFiles(serverpath, string.Format("*{0}{1}", environment, CommonConst.CONFIG_FILE_EXTENSION));

                foreach (var filePath in envFiles)
                {
                    var fi = new FileInfo(filePath);
                    var collectionName = fi.Name.Replace(fi.Extension, "").Replace(string.Format(".{0}", environment), "");
                    JArray arrData = JObjectHelper.GetJArrayFromFile(fi.FullName);
                    _dbProxy.Collection = collectionName;
                    WriteInstallData(arrData, collectionName);
                }
            }

            // Install custom configs
            WriteConfigFileToDB(GetCustomConfigDirectoryPath(), false);
            _logger.Debug("END RunInstallScripts");
        }

        private void WriteConfigFileToDB(string serverpath, bool cleanExistingData = true)
        {
            string[] files = Directory.GetFiles(serverpath, string.Format("*{0}", CommonConst.CONFIG_FILE_EXTENSION));

            foreach (var filePath in files)
            {
                try
                {
                    _logger.Info(string.Format("Installing {0} collection", filePath));

                    var fi = new FileInfo(filePath);
                    var collectionName = fi.Name.Replace(fi.Extension, "");
                    if (collectionName.Contains("."))
                    {
                        continue;
                    }
                    JArray arrData = JObjectHelper.GetJArrayFromFile(fi.FullName);
                    _dbProxy.Collection = collectionName;
                    if (cleanExistingData)
                    {
                        _dbProxy.Delete(CommonConst.EMPTY_JSON_OBJECT);
                    }
                    WriteInstallData(arrData, collectionName);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error in installing {0} collection. Error {1}", filePath, ex.Message), ex);
                }
            }
        }

        private void WriteInstallData(JArray arrData, string collection, string moduleName = "ZApp")
        {
            _dbProxy.Collection = collection;
            foreach (JObject item in arrData)
            {
                var key = item[CommonConst.CommonField.DATA_KEY];
                if (item[CommonConst.CommonField.DISPLAY_ID] == null)
                {
                    item[CommonConst.CommonField.DISPLAY_ID] = Guid.NewGuid().ToString();
                }

                item[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = DateTime.Now;
                item[CommonConst.CommonField.DATA_MODULE_NAME] = moduleName;
                item[CommonConst.CommonField.ÌS_OVERRIDE] = false;
                item[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;

                if (key != null)
                {
                    string updateFilter = "{" + CommonConst.CommonField.DATA_KEY + ":'" + key.ToString() + "'}";
                    _dbProxy.Update(updateFilter, item, true);
                }
                else
                {
                    _dbProxy.WriteData(item);
                }
            }
        }

        private string GetResourceName(ref string requestResource)
        {
            requestResource = requestResource.Replace("/", "");
            if (string.IsNullOrEmpty(requestResource))
            {
                requestResource = defaultResourceName;
            }
            return requestResource.Replace(".", "_").ToLower();
        }

        private JObject CheckAccess()
        {
            var response = _dataBuilderHelper.GetResponseObject(CommonConst._200_OK);
            _dataBuilderHelper.AddDataToArray(response, JObject.Parse("{'type' : 'file_access',  'status' :  " + CheckFileAccess().ToString().ToLower() + "}"))
                .AddDataToArray(response, JObject.Parse("{'type' : 'mongo_db_access',  'status'  :  " + MongoDBConnection().ToString().ToLower() + "}"));

            IsPrerequisiteCheck = true;
            return response;
        }

        private bool CheckFileAccess()
        {
            try
            {
                var tempFolder = ApplicationConfig.AppTempFolder;
                File.WriteAllText(string.Format("{0}\\{1}", tempFolder, "ping.txt"), string.Format("ping-{0}", DateTime.Now.ToString()));
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Check file Access error {0}", ex.Message), ex);
                return false;
            }
        }

        private bool MongoDBConnection()
        {
            return _pingService.PingDb();
        }

        private void UpdateInstallStatus(AppInstallStatus installStatus)
        {
            _status = installStatus;
            JObject installStatusObj = new JObject();
            installStatusObj[CommonConst.CommonField.UPDATED_DATE_TIME] = DateTime.Now;
            installStatusObj[CommonConst.CommonField.STATUS] = (int)installStatus;
            installStatusObj[CommonConst.CommonField.TRANSATTION_ID] = _logger.TransactionId;

            var tempFolder = ApplicationConfig.AppTempFolder;
            JObjectHelper.WriteJSONData(string.Format("{0}\\{1}", tempFolder, _installStatusFile), installStatusObj);
        }

        private void GetInstallStatus()
        {
            int status = 0;

            try
            {
                var tempFolder = ApplicationConfig.AppTempFolder;
                JObject installStatusObj = JObjectHelper.GetJObjectFromFile(string.Format("{0}\\{1}", tempFolder, _installStatusFile));

                if (installStatusObj[CommonConst.CommonField.STATUS] != null)
                {
                    int.TryParse(installStatusObj[CommonConst.CommonField.STATUS].ToString(), out status);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.Info(string.Format("File Not found for getting status {0}", ex.Message));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Format("Error while getting GetInstallStatus {0}", ex.Message), ex);
            }
            _status = (AppInstallStatus)status;
        }
    }
}