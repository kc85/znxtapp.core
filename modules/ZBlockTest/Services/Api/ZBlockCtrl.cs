using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;

namespace ZBlockTest.Services.Api
{
    public class ZBlockCtrl : ZNxtApp.Core.Services.ApiBaseService
    {
        const string ZBLOCK_COLLECTION = "zblock";
       public const string PAGE = "page";
       const string PAGES = "pages";
        const string INDEX = "index";
        const string BLOCK_PATH = "block_path";
        const string DISPLAY_AREA = "display_area";
        private ParamContainer _paramContainer;
        public ZBlockCtrl(ParamContainer paramContainer)
            : base(paramContainer)
        {
            _paramContainer = paramContainer;
        }

        public JObject GetBlocks()
        {
            try
            {
                
                JObject filter = new JObject();
                filter[CommonConst.CommonField.IS_ENABLED] = true;
                string filterPage = string.Empty;
                var page = _paramContainer.GetKey(PAGE);
                if (page != null)
                {
                    filterPage = page.ToString();
                }
                var data = DBProxy.Get(ZBLOCK_COLLECTION, filter.ToString(), new List<string>() { 
                     CommonConst.CommonField.DATA_KEY, PAGES,INDEX, BLOCK_PATH,DISPLAY_AREA
                });
                data = new JArray(data.Where(f => f[PAGES]!=null && (f[PAGES] as JArray).Where(f1=>f1.ToString() == filterPage).Count() != 0).OrderBy(f => (int)f[INDEX]));
                Logger.Debug("Enter to Get Blocks");
                return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, data);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return ResponseBuilder.CreateReponse(CommonConst._500_SERVER_ERROR);
            }
        }
    }
}
