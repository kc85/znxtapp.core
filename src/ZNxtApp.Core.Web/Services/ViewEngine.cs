using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Web.Services
{
    public class ViewEngine :IViewEngine
    {
        private static object _lock = new object();
        private static ViewEngine _viewEngine;
        private bool isDevEnv = false;
        
        private ViewEngine()
        {
            var config = new TemplateServiceConfiguration();
            config.Language = Language.CSharp;
            config.DisableTempFileLocking = true;
            config.CachingProvider = new DefaultCachingProvider(t => { });
#if DEBUG
            config.Debug = true;
            config.DisableTempFileLocking = false;
            isDevEnv = true;
#endif
            Engine.Razor = RazorEngineService.Create(config);
        }
        public static ViewEngine GetEngine()
        {
            if (_viewEngine == null)
            {
                lock (_lock)
                {
                    _viewEngine = new ViewEngine();

                    return _viewEngine;
                }
            }
            else
            {
                return _viewEngine;
            }
        }
        public string Compile(string inputTemplete, string key, object dataModel)
        {
            try
            {
                string result = Engine.Razor.RunCompile(inputTemplete, key, null, dataModel);
                return result;
            }
            catch (Exception ex)
            {
                if (isDevEnv)
                {
                    return ex.Message;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
