using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class PingService : IPingService
    {
        private IDBService _dbService;

        public PingService(IDBService dbService)
        {
            _dbService = dbService;
        }

        public bool PingDb()
        {
            try
            {
                Task t = new Task(() =>
                {
                    _dbService.WriteData(CommonConst.Collection.PING, JObject.Parse("{'ping' : 1}"));
                });

                t.Start();
                t.Wait(500);
                if (!t.IsCompleted)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}