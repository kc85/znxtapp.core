using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Services
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
            return Exec(route.ExecultAssembly, route.ExecuteType, route.ExecuteMethod, helper);
        }
        public object Exec(string execultAssembly, string executeType, string executeMethod, ParamContainer helper)
        {
            Type exeType = _assemblyLoader.GetType(execultAssembly, executeType, _logger);
            if (exeType == null)
            {
                string error = string.Format("Execute Type is null for {0} :: {1}", execultAssembly, executeType);
                _logger.Error(error, null);
                throw new Exception(error);
            }
            Type[] argTypes = new Type[] { typeof(string) };
            object[] argValues = new object[] { helper };
            dynamic obj = Activator.CreateInstance(exeType, argValues);
            var methodInfo = exeType.GetMethod(executeMethod);
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
