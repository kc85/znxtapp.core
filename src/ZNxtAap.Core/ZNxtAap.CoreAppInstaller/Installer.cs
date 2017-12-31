using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.CoreAppInstaller
{
    public class Installer : IAppInstaller
    {
        private AppInstallerConfig _config;
        public Installer(AppInstallerConfig config)
        {
            _config = config;
        }
        public bool Install()
        {
            throw new NotImplementedException();
        }
    }
}
