using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZNxtApp.CoreTest
{
    [TestClass]
    public class ConstTest
    {
        [TestMethod]
        public void CommonConst()
        {
            Assert.IsNotNull(ZNxtApp.Core.Consts.CommonConst.Collection.USERS);
            Assert.IsNotNull(ZNxtApp.Core.Consts.CommonConst.CommonField.ID);
            Assert.IsNotNull(ZNxtApp.Core.Consts.CommonConst.HTMLPages.INDEX);
        }
    }
}