using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZNxtApp.CoreTest
{
    internal class Module1 : ZNxtApp.Core.Model.ModuleServiceBase
    {
    }

    [TestClass]
    public class ModelBaseServiceTest
    {
        [TestMethod]
        public void CreateInstance()
        {
            Assert.IsNotNull(new Module1());
        }
    }
}