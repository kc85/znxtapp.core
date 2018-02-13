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
        private readonly IDBService _dbProxy;

        private Installer(IPingService pingService, DataBuilderHelper dataBuilderHelper, ILogger logger, IDBService dbProxy)
        {
            _pingService = pingService;
            _dataBuilderHelper = dataBuilderHelper;
            _logger = logger;
            _dbProxy = dbProxy;
            GetInstallStatus();
        }

        public static IAppInstaller GetInstance(IPingService pingService, DataBuilderHelper dataBuilderHelper, ILogger logger, IDBService dbProxy)
        {
            if (_appInstaller == null)
            {
                lock (lockObject)
                {
                    _appInstaller = new Installer(pingService, dataBuilderHelper, logger, dbProxy);
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

            RunInstallScripts();

            UpdateInstallStatus(AppInstallStatus.Finish);
            httpProxy.SetResponse(CommonConst._200_OK, GetStatus());
            httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
        }

        private void RunInstallScripts()
        {
            _logger.Debug("START RunInstallScripts");

            Thread.Sleep(1000 * 5);

            var serverpath = string.Format("{0}\\InstallScripts\\Collections", ApplicationConfig.AppBinPath);
            var environment = CommonUtility.GetAppConfigValue(CommonConst.ENVIRONMENT_SETTING_KEY);
            environment = environment == null ? string.Empty : environment;
            string envExtension = string.Format(".{0}{1}", environment, CommonConst.CONFIG_FILE_EXTENSION);
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
                    _dbProxy.Delete(CommonConst.EMPTY_JSON_OBJECT);
                    WriteInstallData(arrData, collectionName);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("Error in installing {0} collection. Error {1}", filePath, ex.Message), ex);
                }
            }

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
            _logger.Debug("END RunInstallScripts");
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