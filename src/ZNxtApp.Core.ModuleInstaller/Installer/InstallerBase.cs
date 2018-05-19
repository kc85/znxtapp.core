using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.ModuleInstaller.Installer
{
    public abstract class InstallerBase
    {
        protected readonly ILogger _logger;
        protected IHttpContextProxy _httpProxy;
        protected readonly IDBService _dbProxy;

        public InstallerBase(ILogger logger, IDBService dbProxy)
        {
            _logger = logger;
            _dbProxy = dbProxy;
        }

        protected void CleanDBCollection(string moduleName, string collection)
        {
            string cleanupFilter = "{ " + CommonConst.CommonField.MODULE_NAME + ":'" + moduleName + "'}";
            
            _dbProxy.Delete(collection,cleanupFilter);
        }

        protected JObject GetModule(JObject moduleObject)
        {            
            var data = _dbProxy.Get(CommonConst.Collection.MODULES,moduleObject.ToString());
            if (data.Count == 0)
            {
                return null;
            }
            else
            {
                return data[0] as JObject;
            }
        }
        protected string GetModuleName(string moduleFullName)
        {
            return moduleFullName.Split('/')[0];
        }

        protected string GetModuleVersion(string moduleFullName)
        {
            var data =  moduleFullName.Split('/');
            if (data.Length != 0)
            {
                return data[1];
            }
            else
            {
                return string.Empty;
            }
        }
    }
}