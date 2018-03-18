﻿using System;
using System.Configuration;
using System.IO;

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
                return ConfigurationManager.AppSettings["BackendPath"];
            }
        }
        public static string AppDefaultPage
        {
            get
            {
                return ConfigurationManager.AppSettings["DefaultPage"];
            }
        }
        public static string TempFolder
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

        public static ApplicationMode GetApplicationMode
        {
            get
            {
                string appMode = ConfigurationManager.AppSettings["AppMode"];
                ApplicationMode appModeEnum = ApplicationMode.Maintance;
                Enum.TryParse<ApplicationMode>(appMode, out appModeEnum);
                return appModeEnum;
            }
        }

        public static string AuthToken { get; set; }

        public static double SessionDuration { get { return 30; } }

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
                //var temppath = string.Format("{0}\\{1}\\{2}", Environment.GetEnvironmentVariable("TEMP"), AppName, AppID);

                //var di = new DirectoryInfo(temppath);
                //if (!di.Exists)
                //{
                //    di.Create();
                //}
                //return temppath;
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
    }

    public enum ApplicationMode
    {
        Maintance,
        Debug,
        Live
    }
}