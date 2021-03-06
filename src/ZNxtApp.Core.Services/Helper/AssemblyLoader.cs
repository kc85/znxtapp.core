﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Services.Helper
{
    public class AssemblyLoader
    {
        private static object _lock = new object();
        private static AssemblyLoader _assemblyLoader;
        private IDBService _dbProxy;
        public Dictionary<string, byte[]> _loadedAssembly = new Dictionary<string, byte[]>();

        private AssemblyLoader()
        {
            _dbProxy = ApplicationConfig.DependencyResolver.GetInstance<IDBService>();
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

        public Type GetType(string assemblyName, string executeType, ILogger logger)
        {
            logger.Info(string.Format("GetType: {0}, executeType: {1}", assemblyName, executeType));
            var assembly = Load(assemblyName.Trim(), logger);
            return assembly.GetType(executeType.Trim());
        }

        public Assembly Load(string assemblyName, ILogger logger)
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
                    assemblyBytes = GetAsssemblyFromDB(assemblyName, logger);
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
            logger.Info(string.Format("Loading Assemmbly:{0}, from Download ", assemblyName));

            var dataResponse = _dbProxy.Get(CommonConst.Collection.DLLS, GetFilter(assemblyName));

            if (dataResponse.Count > 0)
            {
                var assemblyData = dataResponse[0][CommonConst.CommonField.DATA].ToString();
                return System.Convert.FromBase64String(assemblyData);
            }
            return null;
        }

        private static string GetFilter(string path)
        {
            return "{ $and: [ { is_override:{ $ne: true}  }, {'" + CommonConst.CommonField.FILE_PATH + "':  {$regex :'^" + path.ToLower() + "$','$options' : 'i'}}] }";
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