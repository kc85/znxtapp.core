using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZNxtApp.Core.Config;

namespace ZNxtApp.CoreTest
{
    [TestClass]
    public class AppInstallConfig
    {
        [TestMethod]
        public void AppConfigInit()
        {
            AppInstallerConfig installConfig = new AppInstallerConfig() { InstallType = AppInstallType.New };
            Assert.IsNotNull(installConfig);
        }
    }
}