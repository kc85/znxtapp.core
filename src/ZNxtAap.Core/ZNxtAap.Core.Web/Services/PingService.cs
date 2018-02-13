using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class PingService : IPingService
    {
        private IDBService _dbService;

        public PingService(IDBService dbService)
        {
            _dbService = dbService;
            _dbService.Collection = CommonConst.Collection.PING;
        }

        public bool PingDb()
        {
            try
            {
                Task t = new Task(() =>
                {
                    _dbService.WriteData(JObject.Parse("{'ping' : 1}"));
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