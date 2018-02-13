using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZNxtAap.CoreTest
{
    [TestClass]
    public class ConstTest
    {
        [TestMethod]
        public void CommonConst()
        {
            Assert.IsNotNull(ZNxtAap.Core.Consts.CommonConst.Collection.USERS);
            Assert.IsNotNull(ZNxtAap.Core.Consts.CommonConst.CommonField.ID);
            Assert.IsNotNull(ZNxtAap.Core.Consts.CommonConst.HTMLPages.INDEX);
        }
    }
}