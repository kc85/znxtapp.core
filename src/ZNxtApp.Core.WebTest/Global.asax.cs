using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ZNxtApp.Core.Web.AppStart;

namespace ZNxtApp.Core.WebTest
{
    public class Global : GlobalAsaxBase
    {

        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

        }
    }
}