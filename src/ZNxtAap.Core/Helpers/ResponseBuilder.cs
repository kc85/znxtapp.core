using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtAap.Core.Config;
using ZNxtAap.Core.Consts;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.Core.Helpers
{
    public class ResponseBuilder
    {

        private readonly ILogger _logger;
        public ILogReader _logReader;
        private readonly IInitData _initData;

        public ResponseBuilder(ILogger logger, ILogReader logReader, IInitData initData)
        {
            _logger = logger;
            _initData = initData;
            _logReader = logReader;
        }
        public JObject CreateReponse(int code, JToken data = null, JObject extraData = null)
        {
            var response = CreateReponse(code);
            if (extraData != null)
            {
                foreach (var item in extraData)
                {
                    response[item.Key] = item.Value;
                }
            }
            if (data != null)
            {
                response[CommonConst.CommonField.HTTP_RESPONE_DATA] = data;
            }
            return response;
        }
        //public static JObject CreateResponse(int code, string messageText, JArray data = null, JObject extraData = null)
        //{
        //    var response = CreateResponse(code, messageText);
        //    if (extraData != null)
        //    {
        //        foreach (var item in extraData)
        //        {
        //            response[item.Key] = item.Value;
        //        }
        //    }
        //    if (data != null)
        //    {
        //        response[CommonConsts.GETDATA_DATA_NODE_KEY] = data;
        //    }
        //    return response;
        //}
        //public static JObject CreateReponse(int code, string messageText)
        //{
        //    JObject response = new JObject();
        //    response[CommonConst.CommonField.HTTP_RESPONE_CODE] = code;
        //    response[CommonConst.CommonField.HTTP_RESPONE_MESSAGE] = messageText;
        //    return response;
        //}
        public  JObject CreateReponse(int code)
        {
            JObject response = new JObject();
            response[CommonConst.CommonField.HTTP_RESPONE_CODE] = code;
            response[CommonConst.CommonField.HTTP_RESPONE_MESSAGE] = CommonConst.Messages[code];
            response[CommonConst.CommonField.HTTP_RESPONE_TRANSACTION_ID] = _initData.TransactionId;

            if (ApplicationMode.Maintance == ApplicationConfig.GetApplicationMode)
            {
                JObject objDebugData = new JObject();
                objDebugData[CommonConst.CommonValue.TIME_SPAN] = (DateTime.Now - _initData.InitDateTime).TotalMilliseconds;
                objDebugData[CommonConst.CommonValue.LOGS] = _logReader.GetLogs(_initData.TransactionId);
                response[CommonConst.CommonField.HTTP_RESPONE_DEBUG_INFO] = objDebugData;

            }
            return response;
        }
    }
}
