namespace ZNxtAap.Core.Config
{
    public class AppInstallerConfig
    {
        public AppInstallType InstallType;

        public string Name { get; set; }

        public string AdminAccount { get; set; }

        public string AdminPassword { get; set; }
    }

    public enum AppInstallType
    {
        New = 0,
        Reset = 1
    }
}