using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Services
{
    public class PingService :IPingService
    {
        IDBService _dbService;
        public PingService(IDBService dbService)
        {
            _dbService = dbService;
            _dbService.Collection = CommonConst.Collection.PING;
        }
        public bool PingDb()
        {
            try
            {
                _dbService.WriteData(JObject.Parse("{'ping' : 1}"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
