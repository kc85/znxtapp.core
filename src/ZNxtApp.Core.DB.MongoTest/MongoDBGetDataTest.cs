using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Services.Helper;

namespace ZNxtApp.Core.DB.MongoTest
{
    [TestClass]
    public class MongoDBGetDataTest
    {
        private const string CollectionName = "Test";

        private IDependencyRegister _dependencyRegister;

        public MongoDBGetDataTest()
        {
            _dependencyRegister = new UnityDependencyRegister();
            ApplicationConfig.SetDependencyResolver(_dependencyRegister.GetResolver());
            _dependencyRegister.Register<IDBService, MongoDBService>();
            _dependencyRegister.Register<IJSONValidator, JSONValidator>();
            _dependencyRegister.RegisterInstance<IAppSettingService>(AppSettingService.Instance);
            _dependencyRegister.Register<IEncryption, EncryptionService>();
            _dependencyRegister.RegisterInstance<IViewEngine>(RazorTemplateEngine.GetEngine());
            _dependencyRegister.Register<IKeyValueStorage, FileKeyValueFileStorage>();
        }

        private IDBService GetDBInstance()
        {
            return _dependencyRegister.GetResolver().GetInstance<IDBService>();
        }

        [TestMethod]
        public void GetDataFromMongoDB_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","Khanin")
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_AND_Condition_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","Khanin", FilterOperator.Equal, FilterCondition.AND),
                       new Filter("address","Pune", FilterOperator.Equal)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_OR_Condition_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","Khanin", FilterOperator.Equal, FilterCondition.OR),
                       new Filter("address","Pune", FilterOperator.Equal)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_EQ_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","Khanin", FilterOperator.Equal, FilterCondition.AND),
                       new Filter("address","Pune", FilterOperator.Equal)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_NOT_EQ_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("address","Pune", FilterOperator.NotEqual)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_IN_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("address",new List<string>(){"Pune","Assam" }, FilterOperator.In)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_NOT_IN_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("address",new List<string>(){"Pune","Assam" }, FilterOperator.NotIn)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Pune"
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["address"] = "Assam"
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 0);
        }

        [TestMethod]
        public void GetDataFromMongoDB_LessThan_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("age","10", FilterOperator.LessThan)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 9
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_LessThanEqual_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("age","10", FilterOperator.LessThanEquals)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 9
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_GreaterThan_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("age","10", FilterOperator.GreaterThan)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_GreaterThanEquals_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("age","10", FilterOperator.GreaterThanEquals)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_EQ_IgnoreCase_False_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","khanin", FilterOperator.Equal)
                 }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 1);
        }

        [TestMethod]
        public void GetDataFromMongoDB_EQ_IgnoreCase_True_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name","khanin", FilterOperator.Equal)
                 }
            };
            query.Filters.First().Field.IgnoreCase = true;

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "Khanin",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "khanin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_IN_IgnoreCase_True_Operator_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery()
                 {
                       new Filter("name", new List<string>{ "khanin","chou" }, FilterOperator.In)
                 }
            };
            query.Filters.First().Field.IgnoreCase = true;

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "CHou",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "khAnin",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 2);
        }

        [TestMethod]
        public void GetDataFromMongoDB_SortBy_ASC_Number_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery(),
                SortBy = new List<SortField>() {
                    new SortField("age", SortType.ASC)
                }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "D",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "F",
                ["age"] = 100
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 3);
            Assert.AreEqual(dbData[0]["age"].ToString(), "10");
            Assert.AreEqual(dbData[1]["age"].ToString(), "11");
            Assert.AreEqual(dbData[2]["age"].ToString(), "100");
        }

        [TestMethod]
        public void GetDataFromMongoDB_SortBy_ASC_String_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery(),
                SortBy = new List<SortField>() {
                    new SortField("name", SortType.ASC)
                }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "D",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "F",
                ["age"] = 100
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 3);
            Assert.AreEqual(dbData[0]["name"].ToString(), "D");
            Assert.AreEqual(dbData[1]["name"].ToString(), "F");
            Assert.AreEqual(dbData[2]["name"].ToString(), "X");
        }

        [TestMethod]
        public void GetDataFromMongoDB_SortBy_DESC_Number_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery(),
                SortBy = new List<SortField>() {
                    new SortField("age", SortType.DESC)
                }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "D",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "F",
                ["age"] = 100
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 3);
            Assert.AreEqual(dbData[0]["age"].ToString(), "100");
            Assert.AreEqual(dbData[1]["age"].ToString(), "11");
            Assert.AreEqual(dbData[2]["age"].ToString(), "10");
        }

        [TestMethod]
        public void GetDataFromMongoDB_SortBy_DESC_String_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery(),
                SortBy = new List<SortField>() {
                    new SortField("name", SortType.DESC )
                }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "D",
                ["age"] = 10
            };

            dbService.WriteData(CollectionName, data, false);

            data = new JObject
            {
                ["name"] = "F",
                ["age"] = 100
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData.Count, 3);
            Assert.AreEqual(dbData[0]["name"].ToString(), "X");
            Assert.AreEqual(dbData[1]["name"].ToString(), "F");
            Assert.AreEqual(dbData[2]["name"].ToString(), "D");
        }

        [TestMethod]
        public void GetDataFromMongoDB_Projection_Success()
        {
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery(),
                Fields = new List<Field>()
               {
                   new Field("name"),
                     new Field("address.pin")
               }
            };

            IDBService dbService = GetDBInstance();
            dbService.Delete(CollectionName, query.Filters);

            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11,
                ["address"] = new JObject { ["pin"] = "123", ["street"] = "Baner" }
            };

            dbService.WriteData(CollectionName, data, false);

            var dbData = dbService.Get(CollectionName, query);

            Assert.AreEqual(dbData[0].Count(), 2);
            Assert.AreNotEqual(dbData[0]["name"], null);
            Assert.AreNotEqual(dbData[0]["address"], null);
            Assert.AreNotEqual(dbData[0]["address"]["pin"], null);
            Assert.AreEqual(dbData[0]["age"], null);
        }
    }
}