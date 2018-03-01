using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public class EventSubscriberCompletedBaseService : BaseService
    {
        protected object APIRsponse { get; set; }
        public EventSubscriberCompletedBaseService(ParamContainer paramContainer)
            : base(paramContainer)
        {
            APIRsponse = paramContainer.GetKey(CommonConst.CommonValue.PARAM_API_RESPONSE);
        }
        protected bool IsSuccessResponse()
        {

            if (HttpProxy.ResponseStatusCode == CommonConst._200_OK)
            {
                if(APIRsponse is JObject)
                {
                    var response = (APIRsponse as JObject);
                    if (response[CommonConst.CommonField.HTTP_RESPONE_CODE].ToString() == CommonConst._1_SUCCESS.ToString())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
