using System;
using System.Configuration;
using System.IO;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Config
{
    public static class ApplicationConfig
    {
        public static string AppName
        {
            get
            {
                return ConfigurationManager.AppSettings["AppName"];
            }
        }

        public static string AppID
        {
            get
            {
                return ConfigurationManager.AppSettings["AppId"];
            }
        }

        public static string MongoDBConnectionString
        {
            get
            {
                return ConfigurationManager.AppSettings["MongoDBConnectionString"];
            }
        }

        public static string DataBaseName
        {
            get
            {
                return ConfigurationManager.AppSettings["DataBaseName"];
            }
        }

        public static string AppPath
        {
            get
            {
                return ConfigurationManager.AppSettings["AppPath"];
            }
        }

        public static string AppBackendPath
        {
            get
            {
                return (ConfigurationManager.AppSettings["BackendPath"] == null ?
                    "/admin001" : ConfigurationManager.AppSettings["BackendPath"]);
            }
        }

        public static string AppDefaultPage
        {
            get
            {
                return (ConfigurationManager.AppSettings["DefaultPage"] == null ? "/index.z" : ConfigurationManager.AppSettings["DefaultPage"]);
            }
        }

        public static string SystemTempFolder
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + AppName;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string AppTempFolderPath
        {
            get
            {
                var tempFolder = string.Format("{0}\\{1}", ApplicationConfig.AppInstallFolder, ApplicationConfig.AppID);

                return tempFolder;
            }
        }

        public static ApplicationMode GetApplicationMode
        {
            get
            {
                string appMode = ConfigurationManager.AppSettings["AppMode"];
                ApplicationMode appModeEnum = ApplicationMode.Maintenance;
                Enum.TryParse<ApplicationMode>(appMode, out appModeEnum);
                return appModeEnum;
            }
        }

        public static string AuthToken { get; set; }

        private static double _sessionDuration = 30;

        public static double SessionDuration { get { return _sessionDuration; } set { _sessionDuration = value; } }

        public static string AppBinPath { get; set; }

        public static string AppWWWRootPath { get; set; }

        public static string AppModulePath
        {
            get
            {
                return ConfigurationManager.AppSettings["ModuleCachePath"];
            }
        }

        public static string AppInstallFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ModuleCachePath"];
            }
        }

        public static bool StaticContentCache
        {
            get
            {
                bool result = false;
                bool.TryParse(ConfigurationManager.AppSettings["StaticContentCache"], out result);
                return result;
            }
        }
        private static IDependencyResolver _dependencyResolver;

        public static void SetDependencyResolver (IDependencyResolver dependencyResolver) {
            _dependencyResolver = dependencyResolver;
        }
        public static  IDependencyResolver DependencyResolver { get { return _dependencyResolver; } }
    }

    public enum ApplicationMode
    {
        Maintenance,
        Debug,
        Live
    }
}