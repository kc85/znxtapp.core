using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.CoreAppInstaller;

namespace ZNxtAap.CoreAppInstallerTest
{
    [TestClass]
    public class AppInstaller
    {
        [TestMethod]
        public void InitAppInstaller()
        {
            IAppInstaller installer = new Installer(new Core.Config.AppInstallerConfig() { InstallType = Core.Config.AppInstallerConfig.AppInstallType.New });
            Assert.IsNotNull(installer);
        }
    }
}
