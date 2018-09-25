using Newtonsoft.Json.Linq;
using System;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Helpers
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
            var response = CreateResponseObject(code);
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
            AddDebugData(response);
            return response;
        }

        public JObject CreateReponse(int code)
        {
            var response = CreateResponseObject(code);
            AddDebugData(response);
            return response;
        }

        private JObject CreateResponseObject(int code)
        {
            JObject response = new JObject();
            response[CommonConst.CommonField.HTTP_RESPONE_CODE] = code;
            response[CommonConst.CommonField.HTTP_RESPONE_MESSAGE] = CommonConst.Messages[code];
            response[CommonConst.CommonField.TRANSACTION_ID] = _initData.TransactionId;
            return response;
        }

        private void AddDebugData(JObject response)
        {
            if (ApplicationMode.Maintenance == ApplicationConfig.GetApplicationMode)
            {
                JObject objDebugData = new JObject();
                objDebugData[CommonConst.CommonValue.TIME_SPAN] = (DateTime.Now - _initData.InitDateTime).TotalMilliseconds;
                objDebugData[CommonConst.CommonValue.LOGS] = _logReader.GetLogs(_initData.TransactionId);
                response[CommonConst.CommonField.HTTP_RESPONE_DEBUG_INFO] = objDebugData;
            }
        }
    }
}