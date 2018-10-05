using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Module.App.Services.Api.Blocks
{
    public class ZBlockCtrl : ZNxtApp.Core.Services.ApiBaseService
    {
        private const string ZBLOCK_COLLECTION = "zblock";
        public const string PAGE = "page";
        private const string PAGES = "pages";
        private const string INDEX = "index";
        private const string BLOCK_PATH = "block_path";
        private const string DISPLAY_AREA = "display_area";
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
                data = new JArray(data.Where(f => f[PAGES] != null && (f[PAGES] as JArray).Where(f1 => f1.ToString() == filterPage).Count() != 0).OrderBy(f => (int)f[INDEX]));
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