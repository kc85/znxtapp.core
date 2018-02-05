using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Config
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
    }

    public enum ApplicationMode
    {  
        Maintance,
        Debug,
        Live
    }
}
