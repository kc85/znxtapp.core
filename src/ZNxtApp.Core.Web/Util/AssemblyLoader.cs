using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.Util
{
    public class AssemblyLoader
    {
        private static object _lock = new object();
        private static AssemblyLoader _assemblyLoader;
        private IDBService _dbProxy;
        public Dictionary<string, byte[]> _loadedAssembly = new Dictionary<string, byte[]>();
        
        private AssemblyLoader()
        {   
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
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

        public Type GetType(string assemblyName, string executeType,ILogger logger)
        {
            logger.Info(string.Format("GetType: {0}, executeType: {1}", assemblyName, executeType));
            var assembly = Load(assemblyName,logger);
            return assembly.GetType(executeType);
        }

        public Assembly Load(string assemblyName,ILogger logger)
        {
            var assembly = GetFromAppDomain(assemblyName);
            if (assembly == null)
            {
                string localPath = String.Format("{0}{1}", ApplicationConfig.AppBinPath, assemblyName);

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
                    assemblyBytes = GetAsssemblyFromDB(assemblyName,logger);
                    if (assemblyBytes != null)
                    {
                        _loadedAssembly[assemblyName] = assemblyBytes;
                    }
                }
                if (assemblyBytes == null)
                {
                    logger.Error(string.Format("No Assembly found :{0}", assemblyName), null);
                }
                else
                {
                    assembly = Assembly.Load(assemblyBytes);
                }
            }
            return assembly;
        }

        private byte[] GetAsssemblyFromDB(string assemblyName, ILogger logger)
        {
            logger.Info(string.Format("Laoding Assemmbly:{0}, from Download ", assemblyName));

            string filter = "{ " + CommonConst.CommonField.IS_OVERRIDE + " : " + CommonConst.CommonValue.FALSE + ", " + CommonConst.CommonField.FILE_PATH + ":'" + assemblyName.ToLower() + "'}";
            _dbProxy.Collection = CommonConst.Collection.DLLS;
            var dataResponse = _dbProxy.Get(filter);

            if (dataResponse.Count > 0)
            {
                var assemblyData = dataResponse[0][CommonConst.CommonField.DATA].ToString();
                return System.Convert.FromBase64String(assemblyData);
            }
            return null;
        }

        private Assembly GetFromAppDomain(string fullName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if ((assembly.ManifestModule).ScopeName == fullName)
                {
                    return assembly;
                }
            }
            return null;
        }
    }
}