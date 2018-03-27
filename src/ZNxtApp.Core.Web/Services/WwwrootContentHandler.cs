using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Web.Helper;
using ZNxtApp.Core.Web.Util;

namespace ZNxtApp.Core.Web.Services
{
    public class WwwrootContentHandler : IwwwrootContentHandler
    {
        private IHttpContextProxy _httpProxy;
        private IDBService _dbProxy;
        private IViewEngine _viewEngine;
        private ILogger _logger;
        private IActionExecuter _actionExecuter;

        public WwwrootContentHandler(IHttpContextProxy httpProxy, IDBService dbProxy, IViewEngine viewEngine, IActionExecuter actionExecuter, ILogger logger)
        {
            _logger = logger;
            _dbProxy = dbProxy;
            _httpProxy = httpProxy;
            _viewEngine = viewEngine;
            _actionExecuter = actionExecuter;
        }
        public string GetStringContent(string path)
        {
            path = StaticContentHandler.MappedUriPath(path);
            var fi = new FileInfo(path);
            if (fi.Extension == CommonConst.CommonField.SERVER_SIDE_PROCESS_HTML_EXTENSION)
            {
                var response = ServerPageModelHelper.ServerSidePageHandler(path, _dbProxy, _httpProxy, _viewEngine, _actionExecuter, _logger);
                return response;
            }
            else
            {
                var data = StaticContentHandler.GetStringContent(_dbProxy, _logger, path);
                return data;
            }
        }
        public byte[] GetContent(string path)
        {
            path = StaticContentHandler.MappedUriPath(path);
            var data = StaticContentHandler.GetContent(_dbProxy, _logger, path);
            return data;
        }
       
    }
}
