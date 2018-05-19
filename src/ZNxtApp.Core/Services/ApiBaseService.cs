using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Services
{
    public abstract class ApiBaseService : BaseService
    {
        protected RoutingModel Route { get; private set; }
        protected ISessionProvider SessionProvider { get; private set; }
        protected IHttpContextProxy HttpProxy { get; private set; }

        public ApiBaseService(ParamContainer paramContainer)
            : base(paramContainer)
        {
            HttpProxy = paramContainer.GetKey(CommonConst.CommonValue.PARAM_HTTPREQUESTPROXY);
            Route = paramContainer.GetKey(CommonConst.CommonValue.PARAM_ROUTE);
            SessionProvider = paramContainer.GetKey(CommonConst.CommonValue.PARAM_SESSION_PROVIDER);
        }
        protected JObject GetPaggedData(string collection, JArray joins = null, string overrideFilters = null)
        {
            int pageSize = 10, pageSizeData = 0;
            int currentPage = 1, currentPageData = 0;
            if (int.TryParse(HttpProxy.GetQueryString("pagesize"), out pageSizeData))
            {
                pageSize = pageSizeData;
            }

            if (int.TryParse(HttpProxy.GetQueryString("currentpage"), out currentPageData))
            {
                currentPage = currentPageData;
            }
            string filterQuery = HttpProxy.GetQueryString("filter");

            if (string.IsNullOrEmpty(filterQuery))
            {
                filterQuery = CommonConst.EMPTY_JSON_OBJECT;
            }
            if (!string.IsNullOrEmpty(overrideFilters))
            {
                filterQuery = overrideFilters;
            }
            var sort = new Dictionary<string, int>();

            var sortData = HttpProxy.GetQueryString("sort");

            if (sortData != null)
            {
                sort = (Dictionary<string, int>)JsonConvert.DeserializeObject<Dictionary<string, int>>(sortData);
            }
            else
            {
                sort[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = -1;
            }
            List<string> fields = null;

            if (HttpProxy.GetQueryString("fields") != null)
            {
                fields = new List<string>();
                fields.AddRange(HttpProxy.GetQueryString("fields").Split(','));
            }
            return GetPageData(collection, filterQuery, fields, sort, pageSize, currentPage);

            //DoJoins(data, joins);

            //OK();
            //return data;
        }

        protected JObject GetPageData(string collection, string query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1)
        {

            int? top = null;
            int? skip = null;

            top = pageSize;
            skip = (pageSize * (currentPage - 1));
            Logger.Debug(string.Format("GetPageData. Top:{0} Skip:{1} Query:{2}", top, skip, query));
            var dbArrData = DBProxy.Get(collection,query, fields, sortColumns, top, skip);
            JObject extraData = new JObject();

            long count = DBProxy.GetCount(collection, query);
            extraData[CommonConst.CommonField.TOTAL_RECORD_COUNT_KEY] = count;
            extraData[CommonConst.CommonField.TOTAL_PAGES_KEY] = Math.Ceiling(((double)count / pageSize));
            extraData[CommonConst.CommonField.PAGE_SIZE_KEY] = pageSize;
            extraData[CommonConst.CommonField.CURRENT_PAGE_KEY] = currentPage;

            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, dbArrData, extraData);
        }

    }

}
