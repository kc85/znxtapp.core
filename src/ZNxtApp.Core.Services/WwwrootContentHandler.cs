//using ZNxtApp.Core.Helpers;
//using ZNxtApp.Core.Interfaces;
//using ZNxtApp.Core.Services.Helper;

//namespace ZNxtApp.Core.Web.Services
//{
//    public class WwwrootContentHandler : IwwwrootContentHandler
//    {
//        private IHttpContextProxy _httpProxy;
//        private IDBService _dbProxy;
//        private IViewEngine _viewEngine;
//        private ILogger _logger;
//        private IActionExecuter _actionExecuter;

//        public WwwrootContentHandler(IHttpContextProxy httpProxy, IDBService dbProxy, IViewEngine viewEngine, IActionExecuter actionExecuter, ILogger logger)
//        {
//            _logger = logger;
//            _dbProxy = dbProxy;
//            _httpProxy = httpProxy;
//            _viewEngine = viewEngine;
//            _actionExecuter = actionExecuter;
//        }

//        public string GetStringContent(string path)
//        {
//            path = StaticContentHandler.MappedUriPath(path);
//            if (CommonUtility.IsServerSidePage(path))
//            {
//                var response = ServerPageModelHelper.ServerSidePageHandler(path, _dbProxy, _httpProxy, _viewEngine, _actionExecuter, _logger);
//                return response;
//            }
//            else
//            {
//                var data = StaticContentHandler.GetStringContent(_dbProxy, _logger, path);
//                return data;
//            }
//        }

//        public byte[] GetContent(string path)
//        {
//            path = StaticContentHandler.MappedUriPath(path);
//            var data = StaticContentHandler.GetContent(_dbProxy, _logger, path);
//            return data;
//        }
//    }
//}