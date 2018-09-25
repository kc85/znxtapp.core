using Newtonsoft.Json.Linq;
using System;
using System.IO;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api.ModuleInstaller
{
    public class ModuleInstaller : ApiBaseService
    {
        private IModuleInstaller _moduleInstaller;
        private IModuleUninstaller _moduleUninstaller;
        private Func<Func<string, JObject>, JObject> _moduleMethodCaller = null;

        public ModuleInstaller(ParamContainer requestParam) : base(requestParam)
        {
            _moduleInstaller = new ZNxtApp.Core.ModuleInstaller.Installer.ModuleInstaller(Logger, DBProxy);
            _moduleUninstaller = new ZNxtApp.Core.ModuleInstaller.Installer.Uninstaller(Logger, DBProxy);

            _moduleMethodCaller = (Func<string, JObject> methodCall) =>
           {
               try
               {
                   if (ApplicationConfig.GetApplicationMode == ApplicationMode.Maintenance)
                   {
                       var moduleName = HttpProxy.GetQueryString("module_name");

                       if (string.IsNullOrEmpty(moduleName))
                       {
                           Logger.Info("ModuleInstaller module name is empty");
                           return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NAME_EMPTY);
                       }
                       try
                       {
                           var response = methodCall(moduleName);
                           return response;
                       }
                       catch (DirectoryNotFoundException dx)
                       {
                           Logger.Error(string.Format("ModuleInstaller module not found. Erro:{0}", dx.Message), dx);
                           return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                       }
                       catch (FileNotFoundException fx)
                       {
                           Logger.Error(string.Format("ModuleInstaller module config not found. Erro:{0}", fx.Message), fx);
                           return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_CONFIG_MISSING);
                       }
                   }
                   else
                   {
                       Logger.Info("ModuleInstaller.GetModuleDetails MAINTANCE_MODE_OFF");
                       return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MAINTANCE_MODE_OFF);
                   }
               }
               catch (Exception ex)
               {
                   Logger.Error(string.Format("ModuleInstaller, Error:", ex.Message), ex);
                   return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
               }
           };
        }

        public JObject GetModuleDetails()
        {
            return _moduleMethodCaller((string moduleName) =>
            {
                var moduleDetails = _moduleInstaller.GetDetails(moduleName);
                if (moduleDetails != null)
                {
                    HttpProxy.UnloadAppDomain();
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, moduleDetails);
                }
                else
                {
                    Logger.Error(string.Format("Module not found : {0}", moduleName));
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                }
            });
        }

        public JObject Reinstall()
        {
            try
            {
                JObject uninstallResponse = Uninstall();
                JObject installResponse = Install();
                return installResponse;
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("ModuleInstaller.Reinstall, Error:", ex.Message), ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }

        public JObject Install()
        {
            return _moduleMethodCaller((string moduleName) =>
            {
                if (_moduleInstaller.Install(moduleName, HttpProxy, true))
                {
                    HttpProxy.UnloadAppDomain();
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
                else
                {
                    Logger.Error(string.Format("Module install error. Module: {0}", moduleName));
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                }
            });
        }

        public JObject Uninstall()
        {
            return _moduleMethodCaller((string moduleName) =>
            {
                if (_moduleUninstaller.Uninstall(moduleName, HttpProxy))
                {
                    HttpProxy.UnloadAppDomain();
                    return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS);
                }
                else
                {
                    Logger.Error(string.Format("Module uninstall error. Module: {0}", moduleName));
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                }
            });
        }
    }
}