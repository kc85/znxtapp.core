using System.Collections.Generic;
using ZNxtAap.Core.Consts;

namespace ZNxtAap.Core.Model
{
    public class RoutingModel
    {
        public string ContentType { get; set; }
        public string Method { get; set; }
        public string Route { get; set; }
        public string ExecultAssembly { get; set; }
        public string ExecuteType { get; set; }
        public string ExecuteMethod { get; set; }
        public string module { get; set; }
        public List<string> auth_users { get; set; }

        public RoutingModel()
        {
            auth_users = new List<string>();
            ContentType = CommonConst.CONTENT_TYPE_APPLICATION_JSON;
        }

        public string GetJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, Type: {2}, Assembly: {3}, Method: {4}, Module:{5}", Method, Route, ExecuteType, ExecultAssembly, ExecuteMethod,module);
        }
    }
}