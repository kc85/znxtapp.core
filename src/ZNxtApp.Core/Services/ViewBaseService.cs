using ZNxtApp.Core.Model;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Services
{
    public abstract class ViewBaseService : ApiBaseService
    {
        protected IwwwrootContentHandler ContentHandler; 
        public ViewBaseService(ParamContainer paramContainer)
            : base(paramContainer)
        {
            ContentHandler = paramContainer.GetKey(CommonConst.CommonValue.PARAM_CONTENT_HANDLER);
        }
    }
}
