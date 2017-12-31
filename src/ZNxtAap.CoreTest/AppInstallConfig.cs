using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZNxtAap.Core.Config;

namespace ZNxtAap.CoreTest
{
    [TestClass]
    public class AppInstallConfig
    {
        [TestMethod]
        public void AppConfigInit()
        {
            AppInstallerConfig installConfig = new AppInstallerConfig() { InstallType = AppInstallerConfig.AppInstallType.New};
            Assert.IsNotNull(installConfig);
        } 
    }
}
