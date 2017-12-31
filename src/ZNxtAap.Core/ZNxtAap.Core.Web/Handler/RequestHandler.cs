using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ZNxtAap.Core.Web.Handler
{
    public class RequestHandler : RequestHandlerBase
    {
        public override void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusDescription = HttpStatusCode.InternalServerError.ToString();
            context.Response.Write(HttpStatusCode.OK.ToString());
        }
    }
}
