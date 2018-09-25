using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api
{
    public class PingCompletedEventSubscriber : EventSubscriberCompletedBaseService
    {
        public PingCompletedEventSubscriber(ParamContainer paramContainer)
            : base(paramContainer)
        {
        }

        public int PingCompleted()
        {
            if (IsSuccessResponse())
            {
                DBProxy.Delete(CommonConst.Collection.PING, CommonConst.EMPTY_JSON_OBJECT);
            }
            return 1;
        }
    }
}