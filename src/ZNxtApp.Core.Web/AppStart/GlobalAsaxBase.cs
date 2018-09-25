using System;

namespace ZNxtApp.Core.Web.AppStart
{
    public abstract class GlobalAsaxBase : System.Web.HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            InitApp.Run();
        }
    }
}