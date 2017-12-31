using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace ZNxtAap.Core.Web.Handler
{
    public abstract class RequestHandlerBase : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
       // protected IExecuteController _executeController;
        //protected IHttpContextProxy _httpProxy;
        //protected IStaticContentHandler _staticContentHandler;
        // protected ILogger _logger;
        public RequestHandlerBase()
        {

        }
        public abstract void ProcessRequest(HttpContext context);
    }
}
