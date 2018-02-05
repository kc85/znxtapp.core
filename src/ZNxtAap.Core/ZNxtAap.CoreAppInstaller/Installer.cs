using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private AppInstallerConfig _config;
        private AppInstallStatus _status;
        private IPingService _pingService;
        private DataBuilderHelper _dataBuilderHelper;
        private const string PREREQUISITE_STATUS_API = "/install/checkprerequisite";
        private const string CHECK_STATUS_API = "/install/checkstatus";
        private const string INSTALL_START_API = "/install/start";
        private static  IAppInstaller _appInstaller;
        private static bool IsPrerequisiteCheck = false;
        private const string defaultResourceName = "index.html";
        private Installer(IPingService pingService, DataBuilderHelper dataBuilderHelper)
        {
            _pingService = pingService;
            _dataBuilderHelper = dataBuilderHelper;
         
        }
        public static IAppInstaller GetInstance(IPingService pingService, DataBuilderHelper dataBuilderHelper)
        {
            if (_appInstaller == null)
            {
                lock (lockObject)
                {
                    _appInstaller = new Installer(pingService, dataBuilderHelper);
                }
            }
            return _appInstaller;
        }
        private void SetInstallStatus()
        {
            _status = AppInstallStatus.Start;
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
            if (!HandleAPI(httpProxy,requestResource))
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
            SetInstallStatus();
            var requestData = httpProxy.GetRequestBody<AppInstallerConfig>();
            if (requestData == null)
            {
                httpProxy.SetResponse(CommonConst._400_BAD_REQUEST);
                httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
                return;
            }
            _status = AppInstallStatus.Inprogress;
            httpProxy.SetResponse(CommonConst._200_OK, GetStatus());
            httpProxy.ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
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
            _dataBuilderHelper.AddDataToArray(response, JObject.Parse("{'file_access' :  " + CheckFileAccess().ToString().ToLower() + "}"))
                .AddDataToArray(response,  JObject.Parse("{'mongo_db_access' :  " + MongoDBConnection().ToString().ToLower() + "}"));

            IsPrerequisiteCheck = true;
             return response;
        }
        private bool CheckFileAccess()
        {
           // var temppath = Environment.GetEnvironmentVariable("TEMP");
           //  var di = new DirectoryInfo(temppath);
            return false;
        }

        private bool MongoDBConnection()
        {
            return _pingService.PingDb();
        }
    }
}
