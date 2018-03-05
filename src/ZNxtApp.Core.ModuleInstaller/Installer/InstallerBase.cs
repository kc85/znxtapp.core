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
            _dbProxy.Collection = collection;
            _dbProxy.Delete(cleanupFilter);
        }

        protected JObject GetModule(JObject moduleObject)
        {
            _dbProxy.Collection = CommonConst.Collection.MODULES;
            var data = _dbProxy.Get(moduleObject.ToString());
            if (data.Count == 0)
            {
                return null;
            }
            else
            {
                return data[0] as JObject;
            }
        }
    }
}