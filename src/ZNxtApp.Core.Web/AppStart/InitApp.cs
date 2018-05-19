using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web;
using System.Web.Caching;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.DB.Mongo;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Services;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.AppStart
{
    public class InitApp
    {
        private static object lockObj = new object();
        private static InitApp _initApp = null;
        private CacheItemRemovedCallback OnCacheRemove = null;
        private ILogger _logger;
        private IDBService _dbProxy;
        private JArray _cronJobs = new JArray();

        private InitApp()
        {
            _logger = Logger.GetLogger(this.GetType().Name, string.Empty);
            _dbProxy = new MongoDBService(ApplicationConfig.DataBaseName);
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
            SetVariables();
            GetCronJob();
            SetCronJob();
            
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
            _cronJobs = _dbProxy.Get(CommonConst.Collection.CRON_JOB,CommonConst.EMPTY_JSON_OBJECT);
        }

        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyLoader loader = AssemblyLoader.GetAssemblyLoader();
            return loader.Load(string.Format("{0}.dll", args.Name.Split(',')[0]), _logger);
        }

        private static void SetVariables()
        {
            ApplicationConfig.AppBinPath = HttpContext.Current.Server.MapPath("~/bin");
            ApplicationConfig.AppWWWRootPath = string.Format(@"{0}\..\{1}", ApplicationConfig.AppBinPath, CommonConst.Collection.STATIC_CONTECT);
        }

        private void AddTask(string name, long seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }
        public void CacheItemRemoved(string key, object val, CacheItemRemovedReason r)
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