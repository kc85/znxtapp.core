using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;

namespace ZNxtAap.Core.Models
{
    public class RoutingModel
    {
        public string ContentType { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string ExecultAssembly { get; set; }
        public string ExecuteType { get; set; }
        public string ExecuteMethod { get; set; }
        public List<string> auth_users { get; set; }
       
        public RoutingModel()
        {
            auth_users = new List<string>();
            ContentType = CommonConst.APPLICATION_JSON_CONNENT_TYPE;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, Type: {2}, Assembly: {3}, Method: {4}", Method, Route, ExecuteType, ExecultAssembly, ExecuteMethod);
        }
    }
}
