using Newtonsoft.Json.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.ModuleInstaller.Installer
{
    public class Uninstaller : InstallerBase, IModuleUninstaller
    {
        public Uninstaller(ILogger logger, IDBService dbProxy)
            : base(logger, dbProxy)
        {
        }

        public bool Uninstall(string moduleFullName, IHttpContextProxy httpProxy)
        {
            _httpProxy = httpProxy;
            string moduleName = GetModuleName(moduleFullName);

            JObject moduleObject = new JObject();
            moduleObject[CommonConst.CommonField.NAME] = moduleName;
            moduleObject = GetModule(moduleObject);
            if (moduleObject == null)
            {
                _logger.Info(string.Format("Module not found :{0}", moduleFullName));
                return false;
            }
            RevertCollections(moduleName, CommonConst.Collection.STATIC_CONTECT, httpProxy);
            RevertCollections(moduleName, CommonConst.Collection.DLLS, httpProxy);
            foreach (var item in moduleObject[CommonConst.MODULE_INSTALL_COLLECTIONS_FOLDER])
            {
                RevertCollections(moduleName, item.ToString(), httpProxy);
            }
            _dbProxy.Collection = CommonConst.Collection.MODULES;
            _dbProxy.Delete(moduleObject.ToString());
            return true;
        }

        private void RevertCollections(string moduleName, string collection, IHttpContextProxy httpProxy)
        {
            CleanDBCollection(moduleName, collection);
            RevertCollectionData(moduleName, collection, httpProxy);
        }

        private void RevertCollectionData(string moduleName, string collection, IHttpContextProxy httpProxy)
        {
            string updateRevertFilter = "{ $and: [ { " + CommonConst.CommonField.IS_OVERRIDE + ":true }, {" + CommonConst.CommonField.OVERRIDE_BY + ":'" + moduleName + "'}] } ";
            JObject updateData = new JObject();
            updateData[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;
            updateData[CommonConst.CommonField.IS_OVERRIDE] = false;
            _dbProxy.Update(updateRevertFilter, updateData, false, MergeArrayHandling.Union);
        }
    }
}