using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Interfaces;
using ZNxtAap.Core.Model;
using ZNxtAap.Core.Web.Util;

namespace ZNxtAap.Core.Web.Services
{
    public class ActionExecuter : IActionExecuter
    {
         private ILogger _logger;
        private AssemblyLoader _assemblyLoader;
        public ActionExecuter(ILogger logger)
        {
            _logger = logger;
            _assemblyLoader = AssemblyLoader.GetAssemblyLoader();

        }

        public object Exec(RoutingModel route, ParamContainer helper)
        {
            Type exeType = _assemblyLoader.GetType(route.ExecultAssembly, route.ExecuteType);
            if (exeType == null)
            {
                string error = string.Format("Exeute Type is null for {0} :: {1}", route.ExecultAssembly, route.ExecuteType);
                _logger.Error(error, null);
                throw new Exception(error);
            }
            Type[] argTypes = new Type[] { typeof(string) };
            object[] argValues = new object[] { helper };
            dynamic obj = Activator.CreateInstance(exeType, argValues);
            var methodInfo = exeType.GetMethod(route.ExecuteMethod);
            if (methodInfo != null)
            {
                return methodInfo.Invoke(obj, null);
            }
            else
            {
                return null;
            }
        }
    }
}
