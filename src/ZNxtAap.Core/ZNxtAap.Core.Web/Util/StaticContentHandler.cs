using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Web.Util
{
    public class StaticContentHandler
    {
        private ILogger _logger;
        //private IWebDBProxy _dbProxy;

        public StaticContentHandler()
        {
            //_logger = Logger.GetLogger(this.GetType().FullName);
            //_dbProxy = new WebDBProxy(_logger);
        }

        public byte[] GetContent(string path)
        {
            if (new FileInfo(path).Extension.ToLower() == CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_EXTENSION)
            {
                return null;
            }
            else
            {
                return null;
               // return StaticContentHelper.GetContent(_dbProxy, _logger, path, HttpContext.Current.Server.MapPath("~/wwwroot"));
            }
        }
    }
}
