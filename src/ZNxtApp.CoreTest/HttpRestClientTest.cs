using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Services;

namespace ZNxtApp.CoreTest
{
    /// <summary>
    /// Summary description for HttpRestClientTest
    /// </summary>
    [TestClass]
    public class HttpRestClientTest
    {
        public HttpRestClientTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        
        [TestMethod]
        public void GetRequest()
        {
            IHttpRestClient http = new HttpRestClient();
            var response = http.Get("https://znxt.app/api/ecomm/product/scan/getlinks?filter={$and : [{source:'amazon.in'}, {is_scan : false}]}&pagesize=100");
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void GetRequest_withQueryString()
        {
            IHttpRestClient http = new HttpRestClient();
            var response = http.Get("http://23.96.83.176:9090/scan/scan?q=AND");
            Assert.IsNotNull(response);
        }
        [TestMethod]
        public void GetRequest_fail()
        {
            try
            {
                IHttpRestClient http = new HttpRestClient();
                var response = http.Get("https://znxt.app/api/abc/abc");
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
            }
           
        }
    }
}
