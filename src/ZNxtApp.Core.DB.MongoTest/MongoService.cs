//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using ZNxtApp.Core.DB.Mongo;
//using ZNxtApp.Core.Exceptions;
//using ZNxtApp.Core.Interfaces;

//namespace ZNxtApp.Core.DB.MongoTest
//{
//    [TestClass]
//    public class MongoService
//    {
//        [TestMethod]
//        public void WriteData()
//        {
//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            string id = Guid.NewGuid().ToString();
//            JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin'}");
//            var result = dbService.WriteData(data);
//            var jsonData = dbService.Get("{'_id':'" + id + "'}");
//            Assert.AreEqual(1, jsonData.Count);
//        }

//        [TestMethod]
//        public void WriteBulkData()
//        {
//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            for (int i = 0; i < 10000; i++)
//            {
//                string id = Guid.NewGuid().ToString();
//                JObject data = JObject.Parse("{'_id':'" + id + "', 'name':'khanin'}");
//                var result = dbService.WriteData(data);
//                Assert.IsTrue(result);
//            }
//        }

//        [TestMethod]
//        [ExpectedException(typeof(DuplicateDBIDException))]
//        public void DuplicateIdError()
//        {
//            IDBService dbService = new MongoDBService("test_db", "testcollection");

//            string id = Guid.NewGuid().ToString();
//            JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin'}");
//            var result = dbService.WriteData(data);
//            result = dbService.WriteData(data);
//        }

//        [TestMethod]
//        public void UpdateData()
//        {
//            string filter = "{'name':'khanin'}";

//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            string id = Guid.NewGuid().ToString();
//            JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin'}");
//            var result = dbService.WriteData(data);
//            data = JObject.Parse("{'name':'khanin','phone' : '1234'}");

//            dbService.Update(filter, data);

//            var jsonData = dbService.Get(filter);

//            Assert.AreEqual("1234", jsonData[0]["phone"].ToString());
//        }

//        [TestMethod]
//        public void GetData()
//        {
//            int tolalCount = 20;
//            int limit = 10;
//            int skipValue = 15;

//            string filter = "{'name':'khanin'}";

//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            dbService.Delete(filter);
//            List<string> ids = new List<string>();
//            for (int i = 0; i < tolalCount; i++)
//            {
//                string id = Guid.NewGuid().ToString();
//                ids.Add(id);
//                JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin'}");
//                var result = dbService.WriteData(data);
//                Assert.IsTrue(result);
//            }

//            var jsonData = dbService.Get("{'id':'" + ids[0] + "'}");

//            Assert.AreEqual(1, jsonData.Count);

//            jsonData = dbService.Get(filter, null, null, limit);

//            Assert.AreEqual(10, jsonData.Count);

//            jsonData = dbService.Get(filter, null, null, limit, 15);

//            Assert.AreEqual((tolalCount - skipValue), jsonData.Count);

//            var count = dbService.GetCount(filter);

//            Assert.AreEqual(tolalCount, count);
//        }

//        [TestMethod]
//        public void GetDataProjection()
//        {
//            int tolalCount = 2;
//            string filter = "{'name':'khanin'}";

//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            dbService.Delete(filter);
//            List<string> ids = new List<string>();
//            for (int i = 0; i < tolalCount; i++)
//            {
//                string id = Guid.NewGuid().ToString();
//                ids.Add(id);
//                JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin', 'ph' : '+9199993003939'}");
//                var result = dbService.WriteData(data);
//                Assert.IsTrue(result);
//            }

//            var dataResult = dbService.Get(filter, new List<string>() { "ph" });

//            foreach (JObject d in dataResult)
//            {
//                Assert.IsNull(d["name"]);
//                Assert.IsNotNull(d["ph"]);
//            }
//        }

//        [TestMethod]
//        public void GetDataSortBy()
//        {
//            int tolalCount = 20;

//            string filter = "{'name':'khanin'}";

//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            dbService.Delete(filter);
//            List<string> ids = new List<string>();
//            for (int i = 0; i < tolalCount; i++)
//            {
//                string id = Guid.NewGuid().ToString();
//                string key = ((i * 1213) % 10).ToString();
//                ids.Add(id);
//                JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin','key' : '" + key + "'}");
//                var result = dbService.WriteData(data);
//                Assert.IsTrue(result);
//            }

//            var jsonData = dbService.Get("{'id':'" + ids[0] + "'}");

//            Assert.AreEqual(1, jsonData.Count);

//            var sort = new Dictionary<string, int>();
//            sort.Add("key", 1);
//            jsonData = dbService.Get(filter, null, sort);
//        }

//        [TestMethod]
//        public void GetDataByFields()
//        {
//            int tolalCount = 20;

//            string filter = "{'name':'khanin'}";

//            IDBService dbService = new MongoDBService("test_db", "testcollection");
//            dbService.Delete(filter);
//            List<string> ids = new List<string>();
//            for (int i = 0; i < tolalCount; i++)
//            {
//                string id = Guid.NewGuid().ToString();
//                string key = ((i * 1213) % 10).ToString();
//                ids.Add(id);
//                JObject data = JObject.Parse("{'id':'" + id + "', 'name':'khanin','key' : '" + key + "', 'info' : {'age' : 10, 'ph' :'" + key + "'} }");
//                var result = dbService.WriteData("a",data);
//                Assert.IsTrue(result);
//            }

//            var jsonData = dbService.Get("a","{'id':'" + ids[0] + "'}", new List<string>() { "name", "info.age" });

//            Assert.AreEqual(1, jsonData.Count);
//        }
//    }
//}