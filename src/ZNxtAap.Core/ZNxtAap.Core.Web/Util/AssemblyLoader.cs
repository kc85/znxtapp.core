using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZNxtAap.Core.Config;

namespace ZNxtAap.Core.Web.Util
{
    public class AssemblyLoader
    {
        private static object _lock = new object();
        private static AssemblyLoader _assemblyLoader;

        //private WebDBProxy _dbProxy;
        public Dictionary<string, byte[]> _loadedAssembly = new Dictionary<string, byte[]>();

        //private Logger _logger;

        private AssemblyLoader()
        {
            //  _logger = Logger.GetLogger(this.GetType().FullName);
            //  _dbProxy = new WebDBProxy(_logger);
        }

        public static AssemblyLoader GetAssemblyLoader()
        {
            if (_assemblyLoader == null)
            {
                lock (_lock)
                {
                    _assemblyLoader = new AssemblyLoader();
                }
            }
            return _assemblyLoader;
        }

        public Type GetType(string assemblyName, string executeType)
        {
            //  _logger.Info(string.Format("GetType: {0}, executeType: {1}", assemblyName, executeType));
            var assembly = Load(assemblyName);
            return assembly.GetType(executeType);
        }

        public Assembly Load(string assemblyName)
        {
            var assembly = GetFromAppDomain(assemblyName);
            if (assembly == null)
            {
                string localPath = String.Format("{0}{1}", ApplicationConfig.AppBinPath, assemblyName);

                // _logger.Info(string.Format("Laoding Assemmbly:{0}, LocalPath : {1}", assemblyName, localPath));
                Byte[] assemblyBytes = null;
                if (_loadedAssembly.ContainsKey(assemblyName))
                {
                    assemblyBytes = _loadedAssembly[assemblyName];
                }
                else if (File.Exists(localPath))
                {
                    assemblyBytes = File.ReadAllBytes(localPath);
                    _loadedAssembly[assemblyName] = assemblyBytes;
                }
                else
                {
                    // assemblyBytes = Download(assemblyName);
                    if (assemblyBytes != null)
                    {
                        _loadedAssembly[assemblyName] = assemblyBytes;
                    }
                }
                if (assemblyBytes == null)
                {
                    //  _logger.Info(string.Format("No Assembly found :{0}", assemblyName));
                }
                else
                {
                    assembly = Assembly.Load(assemblyBytes);
                }
            }
            return assembly;
        }

        //private byte[] Download(string assemblyName)
        //{
        //    _logger.Info(string.Format("Laoding Assemmbly:{0}, from Download ", assemblyName));

        //    string filter = "{ " + CommonConsts.DATA_FILE_PATH_FIELD_NAME + ":'" + assemblyName.ToLower() + "'}";
        //    var dataResponse = _dbProxy.GetData(Common.CommonConsts.DB_COLLECTION_DLLS, filter);
        //    if (dataResponse != null && dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY] != null)
        //    {
        //        if ((dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY] as JArray).Count > 0)
        //        {
        //            var assemblyData = dataResponse[CommonConsts.GETDATA_DATA_NODE_KEY][0][CommonConsts.GETDATA_DATA_NODE_KEY.ToLower()].ToString();
        //            return System.Convert.FromBase64String(assemblyData);
        //        }
        //    }
        //    return null;
        //}

        private Assembly GetFromAppDomain(string fullName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if ((assembly.ManifestModule).ScopeName == fullName)
                {
                    // _logger.Info(string.Format("Find assembly on App Domain : {0}", fullName));
                    return assembly;
                }
            }
            return null;
        }
    }
}