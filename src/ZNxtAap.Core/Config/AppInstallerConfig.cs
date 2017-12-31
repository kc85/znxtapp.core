using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtAap.Core.Config
{
    public class AppInstallerConfig
    {
        public AppInstallType InstallType;

        public enum AppInstallType
        {
            New,
            Reset
        }
    }
}
