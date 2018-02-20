using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Model;

namespace ZNxtAap.Core.Web.Services.Api
{
    public class PingController
    {
        public PingController(ParamContainer requestHelper)
        {

        }

        public string Ping()
        {
            return "1";
        }
    }
}
