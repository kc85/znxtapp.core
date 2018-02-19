using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.ModuleInstaller.Installer
{
    public class Uninstall : InstallerBase,IModuleUninstaller
    {
        public Uninstall(ILogger logger, IDBService dbProxy)
            : base(logger, dbProxy)
        {
        }
        public bool Uninstall(string moduleName, IHttpContextProxy httpProxy)
        {
            _httpProxy = httpProxy;
            RemoveWWWRootFiles(moduleName,httpProxy);
            return true;
        }
        private void RemoveWWWRootFiles(string moduleName, IHttpContextProxy httpProxy)
        {

        }
    }
}
