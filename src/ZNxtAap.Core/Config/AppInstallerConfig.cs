using System.Collections.Generic;
namespace ZNxtAap.Core.Config
{
    public class AppInstallerConfig
    {
        public AppInstallType InstallType;

        public string Name { get; set; }

        public string AdminAccount { get; set; }

        public string AdminPassword { get; set; }
     
        public List<string> DefaultModules { get; set; }
        
        public AppInstallerConfig()
        {
            DefaultModules = new List<string>();
        }
    }

    public enum AppInstallType
    {
        New = 0,
        Reset = 1
    }
}