using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Services.Helper;

namespace ZNxtApp.CoreTest
{
    /// <summary>
    /// Summary description for FileKeyValueTest
    /// </summary>
    [TestClass]
    public class FileKeyValueTest
    {

        private IDependencyRegister _dependencyRegister;

        public FileKeyValueTest()
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

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion Additional test attributes

        [TestMethod]
        public void KeyValuePut_Success()
        {
            var dbname = ApplicationConfig.DataBaseName;
                        
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            Assert.AreEqual(store.Put<string>("test", "v1", "data"), true);
        }

        [TestMethod]
        public void KeyValueGet_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            var data = "data";
            store.Put<string>("test", "v1", data);

            Assert.AreEqual(store.Get<string>("test", "v1"), data);
        }

        [TestMethod]
        public void KeyValueByteArr_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            var data = "data";
            store.Put<byte[]>("test", "v1", Encoding.ASCII.GetBytes(data));

            Assert.AreEqual(Encoding.ASCII.GetString(store.Get<byte[]>("test", "v1")), data);
        }

        [TestMethod]
        public void KeyValueByteArrEncryption_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            var data = "datakhaninChoudhury1212121";
            store.Put<byte[]>("test", "v2", Encoding.ASCII.GetBytes(data), "Encryptionstring%%!&&asdasd!dasd");

            Assert.AreEqual(Encoding.ASCII.GetString(store.Get<byte[]>("test", "v2", "Encryptionstring%%!&&asdasd!dasd")), data);
        }

        [TestMethod]
        public void KeyValueDelete_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            store.Put<string>("test", "v11", "data");
            Assert.AreEqual(store.Delete("test", "v11"), true);
        }

        [TestMethod]
        public void KeyValueDeleteAll_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            store.Put<string>("test", "v11", "data");

            foreach (var item in store.GetKeys("test"))
            {
                store.Delete("test", item);
            }
            Assert.AreEqual(store.GetKeys("test").Count, 0);
        }

        [TestMethod]
        public void KeyValueGetBucket_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            store.Put<string>("test", "v11", "data");
            store.Put<string>("test1", "v11", "data");

            Assert.AreEqual(store.GetBuckets().Count, 2);
        }

        [TestMethod]
        public void KeyValueDeleteBucket_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            foreach (var item in store.GetBuckets())
            {
                store.DeleteBucket(item);
            }

            Assert.AreEqual(store.GetBuckets().Count, 0);
        }
        [TestMethod]
        public void KeyValueEmptyGet_Success()
        {
            IKeyValueStorage store = new FileKeyValueFileStorage(new EncryptionService(), AppSettingService.Instance);

            store.Put<string>("test", "v1","a");

            
            Assert.AreEqual(string.IsNullOrEmpty(store.Get<string>("test", "v11")), true);
        }
    }
}