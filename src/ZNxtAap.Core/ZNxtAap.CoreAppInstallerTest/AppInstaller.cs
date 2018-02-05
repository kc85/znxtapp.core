using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.CoreAppInstaller;
using ZNxtAap.Core.Web.Services;
using ZNxtAap.Core.DB.Mongo;

namespace ZNxtAap.CoreAppInstallerTest
{
    [TestClass]
    public class AppInstaller
    {
        [TestMethod]
        public void InitAppInstaller()
        {
            IAppInstaller installer = Installer.GetInstance(new PingService(new MongoDBService("test_db")), new Core.Helpers.DataBuilderHelper());
            installer.Install(new HttpContextProxyMock());
            Assert.IsNotNull(installer);
        }
    }
}
