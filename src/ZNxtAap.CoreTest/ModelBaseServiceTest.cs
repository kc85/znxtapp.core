using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZNxtAap.CoreTest
{
    class Module1 : ZNxtAap.Core.Module.ModuleServiceBase
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
