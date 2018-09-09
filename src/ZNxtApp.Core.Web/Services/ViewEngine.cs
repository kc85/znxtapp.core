using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
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
            config.EncodedStringFactory = new RawStringFactory(); // Raw string encoding.
            //config.EncodedStringFactory = new HtmlEncodedStringFactory(); // Html encoding.
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
        //public static string IsolatedTransform<Ttype>(string templateId, string templateData, object data) where Ttype : class
        //{
        //    using (var service = IsolatedRazorEngineService.Create(RazorEngineAppDomain))
        //    {
        //        if (service.IsTemplateCached(templateId, typeof(Ttype)))
        //        {
        //            return service.Run(templateId, typeof(Ttype), data);
        //        }
        //        else
        //        {
        //            return service.RunCompile(templateData, templateId, typeof(Ttype), data);
        //        }
        //    }
        //}
        //public static AppDomain RazorEngineAppDomain()
        //{
        //    AppDomainSetup adSetup = new AppDomainSetup();
        //    adSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        //    var current = AppDomain.CurrentDomain;
        //    var strongNames = new StrongName[0];
        //    return AppDomain.CreateDomain(
        //        "RazorEngineDoamin", null,
        //        current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
        //        strongNames);
        //}
        public string Compile(string inputTemplete, string key, object dataModel)
        {
            try
            {
                if (dataModel is Dictionary<string, dynamic>)
                {
                    StringBuilder headerAppender = new StringBuilder();
                    headerAppender.AppendLine("@{");
                    foreach (var item in (dataModel as Dictionary<string, dynamic>))
                    {
                        if (item.Key == CommonConst.CommonValue.METHODS)
                        {
                            foreach (var itemMethod in (item.Value as Dictionary<string, dynamic>))
                            {
                                headerAppender.AppendLine(string.Format("dynamic {0} = @Model[\"{1}\"][\"{0}\"];", itemMethod.Key, CommonConst.CommonValue.METHODS));
                            }
                        }
                    }
                    inputTemplete = headerAppender.AppendLine("}").AppendLine(inputTemplete).ToString();
                }

                if (!Engine.Razor.IsTemplateCached(key, null))
                {
                    return Engine.Razor.RunCompile(inputTemplete, key, null, dataModel);
                }
                else
                {
                    return Engine.Razor.Run(key, null, dataModel);
                }
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
