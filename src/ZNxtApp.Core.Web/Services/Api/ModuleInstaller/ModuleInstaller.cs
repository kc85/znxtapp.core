using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using System.IO;

namespace ZNxtApp.Core.Web.Services.Api.ModuleInstaller
{
    public class ModuleInstaller : ApiBaseService
    {
        IModuleInstaller _moduleInstaller;
        public ModuleInstaller(ParamContainer requestParam) : base(requestParam)
        {
           
            _moduleInstaller = new ZNxtApp.Core.ModuleInstaller.Installer.Installer(Logger, DBProxy);
        }
        public JObject GetModuleDetails()
        {
            if (ApplicationConfig.GetApplicationMode == ApplicationMode.Maintance)
            {
               var moduleName =  HttpProxy.GetQueryString("module_name");

                if (string.IsNullOrEmpty(moduleName))
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NAME_EMPTY);
                }
                try
                {
                    var moduleDetails = _moduleInstaller.GetDetails(moduleName);
                    return ResponseBuilder.CreateReponse(CommonConst._200_OK, moduleDetails);
                }
                catch (DirectoryNotFoundException)
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                }
                catch (FileNotFoundException)
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_CONFIG_MISSING);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MAINTANCE_MODE_OFF);
            }
        }
        public JObject Install()
        {
            if (ApplicationConfig.GetApplicationMode == ApplicationMode.Maintance)
            {
                var moduleName = HttpProxy.GetQueryString("module_name");

                if (string.IsNullOrEmpty(moduleName))
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NAME_EMPTY);
                }
                try
                {
                    if (_moduleInstaller.Install(moduleName, HttpProxy, true))
                    {
                        return ResponseBuilder.CreateReponse(CommonConst._200_OK);
                    }
                    else
                    {
                        return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_NOT_FOUND);
                }
                catch (FileNotFoundException)
                {
                    return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MODULE_CONFIG_MISSING);
                }
            }
            else
            {
                return ResponseBuilder.CreateReponse(ModuleInstallerResponseCode._MAINTANCE_MODE_OFF);
            }
        }
    }
}
