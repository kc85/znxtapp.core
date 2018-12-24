using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZNxtApp.Core.AppInstallerTest
{
    [TestClass]
    public class AppInstaller
    {
        [TestMethod]
        public void InitAppInstaller()
        {
            //IAppInstaller installer = Installer.GetInstance(
            //    new PingService(new MongoDBService("test_db")),
            //    new Core.Helpers.DataBuilderHelper(),
            //    Logger.GetLogger(typeof(Installer).Name),
            //    new MongoDBService("aaa"),
            //    new EncryptionService(),
            //   new ZNxtAap.Core.ModuleInstaller.Installer.Installer(Logger.GetLogger(typeof(Installer).Name)));

            //installer.Install(new HttpContextProxyMock());

            //Assert.IsNotNull(installer);
        }
    }
}