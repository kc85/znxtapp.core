using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.ModuleInstaller.Installer
{
    public abstract class InstallerBase
    {
        protected readonly ILogger _logger;
        protected IHttpContextProxy _httpProxy;
        protected readonly  IDBService _dbProxy;
        public InstallerBase(ILogger logger, IDBService dbProxy)
        {
            _logger = logger;
            _dbProxy = dbProxy;
        }
    }
}
