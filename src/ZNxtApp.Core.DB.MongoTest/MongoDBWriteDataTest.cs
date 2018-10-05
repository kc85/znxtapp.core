using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Services.Helper;


namespace ZNxtApp.Core.DB.MongoTest
{
    [TestClass]
    public class MongoDBWriteDataTest
    {
        const string DBName = "DotNetCoreTest";
        const string CollectionName = "Test";

        IDependencyRegister _dependencyRegister;
        public MongoDBWriteDataTest()
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
        public void WriteDataToMongoDB_Success()
        {
            IDBService dbService = GetDBInstance();
            JObject data = new JObject
            {
                ["name"] = "Khanin"
            };

            Assert.AreEqual(dbService.WriteData(CollectionName, data, false), true);

        }

        [TestMethod]
        public void WriteDataToMongoDB_Schema_Validtion_Success()
        {
            var schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\",    \"type\": \"object\",    \"properties\": {      \"name\": {        \"type\": \"string\"      },      \"age\": {        \"type\": \"integer\"      },      \"address\": {        \"type\": \"object\",        \"properties\": {          \"pin\": {            \"type\": \"integer\"          },          \"street\": {            \"type\": \"string\"          }        },        \"required\": [       \"pin\",    \"street\"     ]   }    },    \"required\": [   \"name\",   \"age\",   \"address\"    ]  }";
            IDBService dbService = GetDBInstance();
            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11,
                ["address"] = new JObject { ["pin"] = 123, ["street"] = "Baner" }
            };

            Assert.AreEqual(dbService.WriteData(CollectionName, data, schema), true);

        }

        [TestMethod]
        [ExpectedException(typeof(SchemaValidationException))]
        public void WriteDataToMongoDB_Schema_Validtion_Fail()
        {
            var schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\",    \"type\": \"object\",    \"properties\": {      \"name\": {        \"type\": \"string\"      },      \"age\": {        \"type\": \"integer\"      },      \"address\": {        \"type\": \"object\",        \"properties\": {          \"pin\": {            \"type\": \"integer\"          },          \"street\": {            \"type\": \"string\"          }        },        \"required\": [       \"pin\",    \"street\"     ]   }    },    \"required\": [   \"name\",   \"age\",   \"address\"    ]  }";
            IDBService dbService = GetDBInstance();
            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = "11",
                ["address"] = new JObject { ["pin"] = 123, ["street"] = "Baner" }
            };

            dbService.WriteData(CollectionName, data, schema);

        }
        [TestMethod]
        public void WriteDataToMongoDB_Schema_Validtion_From_DB_Success()
        {
            var schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\",    \"type\": \"object\",    \"properties\": {      \"name\": {        \"type\": \"string\"      },      \"age\": {        \"type\": \"integer\"      },      \"address\": {        \"type\": \"object\",        \"properties\": {          \"pin\": {            \"type\": \"integer\"          },          \"street\": {            \"type\": \"string\"          }        },        \"required\": [       \"pin\",    \"street\"     ]   }    },    \"required\": [   \"name\",   \"age\",   \"address\"    ]  }";
            IDBService dbService = GetDBInstance();

            dbService.PutSchema(CollectionName, schema);
            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = 11,
                ["address"] = new JObject { ["pin"] = 123, ["street"] = "Baner" }
            };

            dbService.WriteData(CollectionName, data, true);

        }

        [TestMethod]
        [ExpectedException(typeof(SchemaValidationException))]
        public void WriteDataToMongoDB_Schema_Validtion_From_DB_Fail()
        {
            var schema = "{ \"$schema\": \"http://json-schema.org/draft-04/schema#\",    \"type\": \"object\",    \"properties\": {      \"name\": {        \"type\": \"string\"      },      \"age\": {        \"type\": \"integer\"      },      \"address\": {        \"type\": \"object\",        \"properties\": {          \"pin\": {            \"type\": \"integer\"          },          \"street\": {            \"type\": \"string\"          }        },        \"required\": [       \"pin\",    \"street\"     ]   }    },    \"required\": [   \"name\",   \"age\",   \"address\"    ]  }";
            IDBService dbService = GetDBInstance();

            dbService.PutSchema(CollectionName, schema);
            JObject data = new JObject
            {
                ["name"] = "X",
                ["age"] = "11",
                ["address"] = new JObject { ["pin"] = 123, ["street"] = "Baner" }
            };

            dbService.WriteData(CollectionName, data, true);

        }
    }
}
