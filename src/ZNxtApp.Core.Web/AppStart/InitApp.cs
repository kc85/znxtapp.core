using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Services;
using ZNxtApp.Core.Services.Helper;
using ZNxtApp.Core.Web.Services;

namespace ZNxtApp.Core.Web.AppStart
{
    public class InitApp
    {
        private static object lockObj = new object();
        private static InitApp _initApp = null;
        private static IDependencyRegister _dependencyRegister;
        private CacheItemRemovedCallback OnCacheRemove = null;
        private ILogger _logger;
        private IDBService _dbProxy;
        private JArray _cronJobs = new JArray();
        
        private InitApp()
        {
            
        }

        private void InitDB()
        {
            _logger = Logger.GetLogger(this.GetType().Name, string.Empty);
            _dbProxy = new MongoDBService();
        }

        public static void Run()
        {
            if (_initApp == null)
            {
                lock (lockObj)
                {
                    _initApp = new InitApp();
                    _initApp.InitAppRun();
                }
            }
        }

        private void InitAppRun()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            SetDepencencies();
            InitDB();
            SetVariables();
            GetCronJob();
            SetCronJob();
        }
        
        private void SetDepencencies()
        {
            _dependencyRegister = new UnityDependencyRegister();
            ApplicationConfig.SetDependencyResolver(_dependencyRegister.GetResolver());
            _dependencyRegister.Register<IDBService, MongoDBService>();
            _dependencyRegister.Register<IJSONValidator, JSONValidator>();
            _dependencyRegister.RegisterInstance<IAppSettingService>(AppSettingService.Instance);
            _dependencyRegister.Register<IEncryption, EncryptionService>();
            _dependencyRegister.RegisterInstance<IViewEngine>(RazorTemplateEngine.GetEngine());
            _dependencyRegister.Register<IPingService, PingService>();
            _dependencyRegister.Register<IKeyValueStorage, FileKeyValueFileStorage>();
        }

        private void SetCronJob()
        {
            foreach (JObject job in _cronJobs)
            {
                long timeSpan = 0;

                if (job[CommonConst.CommonField.REPEAT_IN] != null && long.TryParse(job[CommonConst.CommonField.REPEAT_IN].ToString(), out timeSpan))
                {
                    AddTask(job[CommonConst.CommonField.DATA_KEY].ToString(), (timeSpan * 60));
                }
            }
        }

        private void GetCronJob()
        {
            _cronJobs = _dbProxy.Get(CommonConst.Collection.CRON_JOB, CommonConst.EMPTY_JSON_OBJECT);
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyLoader loader = AssemblyLoader.GetAssemblyLoader();
            return loader.Load(string.Format("{0}.dll", args.Name.Split(',')[0]), _logger);
        }

        private void SetVariables()
        {
            try
            {
                ApplicationConfig.AppBinPath = HttpContext.Current.Server.MapPath("~/bin");
                ApplicationConfig.AppWWWRootPath = string.Format(@"{0}\..\{1}", ApplicationConfig.AppBinPath, CommonConst.Collection.STATIC_CONTECT);

                double duration = ApplicationConfig.SessionDuration;
                double.TryParse(AppSettingService.Instance.GetAppSettingData(CommonConst.CommonField.SESSION_DURATION), out duration);
                ApplicationConfig.SessionDuration = duration;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private void AddTask(string name, long seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        private void CacheItemRemoved(string key, object val, CacheItemRemovedReason r)
        {
            //Logger.TransactionID = CommonUtility.GenerateTxnId("CJ");
            CronJobExecuter job = new CronJobExecuter();

            var jobData = (_cronJobs.FirstOrDefault(f => f[CommonConst.CommonField.DATA_KEY].ToString() == key) as JObject);
            if (jobData != null)
            {
                _logger.Debug(string.Format("Executing Cron Job {0}", key), jobData);
                job.Exec(jobData);
            }
            AddTask(key, Convert.ToInt32(val));
        }
    }
}