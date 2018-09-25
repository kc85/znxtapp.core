using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Web.Services.Api
{
    public class PingInitEventSubscriber : EventSubscriberInitBaseService
    {
        public PingInitEventSubscriber(ParamContainer paramContainer)
            : base(paramContainer)
        {
        }

        public int PingBeforeStart()
        {
            return 1;
        }
    }
}