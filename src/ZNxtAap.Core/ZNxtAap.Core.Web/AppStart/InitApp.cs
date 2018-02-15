using System.Web;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;

namespace ZNxtAap.Core.Web.AppStart
{
    public class InitApp
    {
        private static object lockObj = new object();
        private static InitApp _initApp = null;

        private InitApp()
        {
        }

        public static void Run()
        {
            if (_initApp == null)
            {
                lock (lockObj)
                {
                    _initApp = new InitApp();
                    _initApp.InitAppRun();
                }
            }
        }

        private void InitAppRun()
        {
            ApplicationConfig.AppBinPath = HttpContext.Current.Server.MapPath("~/bin");
            ApplicationConfig.AppWWWRootPath = string.Format(@"{0}\..\{1}", ApplicationConfig.AppBinPath,CommonConst.Collection.STATIC_CONTECT);
        }
    }
}