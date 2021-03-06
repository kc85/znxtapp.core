﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Helpers;
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

        protected JObject GetPaggedData(string collection, JArray joins = null, string overrideFilters = null, Dictionary<string, int> sortColumns = null, List<string> fields = null)
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

            var sortData = HttpProxy.GetQueryString("sort");

            if (sortData != null)
            {
                sortColumns = (Dictionary<string, int>)JsonConvert.DeserializeObject<Dictionary<string, int>>(sortData);
            }
            else if (sortColumns == null)
            {
                sortColumns = new Dictionary<string, int>();
                sortColumns[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = -1;
            }

            if (fields == null)
            {
                fields = new List<string>();
            }
            if (HttpProxy.GetQueryString("fields") != null)
            {
                fields = new List<string>();
                fields.AddRange(HttpProxy.GetQueryString("fields").Split(','));
            }
            var data = GetPagedData(collection, filterQuery, fields, sortColumns, pageSize, currentPage);

            DoJoins(data, collection, joins);

            //OK();
            return data;
        }

        protected JObject GetCollectionJoin(string soureField, string destinationCollection, string destinationJoinField, List<string> fields, string valueKey)
        {
            JObject collectionJoin = new JObject();
            collectionJoin[CommonConst.CommonField.DB_JOIN_DESTINATION_COLELCTION] = destinationCollection;
            collectionJoin[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELD] = destinationJoinField;
            collectionJoin[CommonConst.CommonField.DB_JOIN_SOURCE_FIELD] = soureField;
            collectionJoin[CommonConst.CommonField.DB_JOIN_VALUE] = valueKey;
            if (fields != null)
            {
                JArray jarrFields = new JArray();
                foreach (var item in fields)
                {
                    jarrFields.Add(item);
                }
                collectionJoin[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELDS] = jarrFields;
            }
            return collectionJoin;
        }

        protected JObject GetPagedData(string collection, string query, List<string> fields = null, Dictionary<string, int> sortColumns = null, int pageSize = 10, int currentPage = 1)
        {
            int? top = null;
            int? skip = null;

            top = pageSize;
            skip = (pageSize * (currentPage - 1));
            Logger.Debug(string.Format("GetPageData. Top:{0} Skip:{1} Query:{2}", top, skip, query));
            var dbArrData = DBProxy.Get(collection, query, fields, sortColumns, top, skip);
            JObject extraData = new JObject();

            long count = DBProxy.GetCount(collection, query);
            extraData[CommonConst.CommonField.TOTAL_RECORD_COUNT_KEY] = count;
            extraData[CommonConst.CommonField.TOTAL_PAGES_KEY] = Math.Ceiling(((double)count / pageSize));
            extraData[CommonConst.CommonField.PAGE_SIZE_KEY] = pageSize;
            extraData[CommonConst.CommonField.CURRENT_PAGE_KEY] = currentPage;

            return ResponseBuilder.CreateReponse(CommonConst._1_SUCCESS, dbArrData, extraData);
        }

        private void DoJoins(JObject data, string sourceCollection, JArray joins)
        {
            if (joins != null)
            {
                Dictionary<string, List<string>> collectionIds = new Dictionary<string, List<string>>();

                // get the join keys
                foreach (JObject join in joins)
                {
                    collectionIds.Add(join[CommonConst.CommonField.DB_JOIN_SOURCE_FIELD].ToString(), new List<string>());
                }

                // get the join ids
                if (data[CommonConst.CommonField.DATA] == null)
                {
                    return;
                }

                foreach (JObject item in data[CommonConst.CommonField.DATA] as JArray)
                {
                    foreach (var joinColumn in collectionIds)
                    {
                        if (item[joinColumn.Key] != null)
                        {
                            joinColumn.Value.Add(item[joinColumn.Key].ToString());
                        }
                    }
                }

                // gte data from IDs
                foreach (var joinCoumnId in collectionIds)
                {
                    var join = joins.FirstOrDefault(f => f[CommonConst.CommonField.DB_JOIN_SOURCE_FIELD].ToString() == joinCoumnId.Key);
                    if (join != null)
                    {
                        List<string> fields = new List<string>();
                        if (join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELDS] != null)
                        {
                            fields.Add(join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELD].ToString());
                            foreach (var field in join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELDS] as JArray)
                            {
                                fields.Add(field.ToString());
                            }
                        }
                        else
                        {
                            fields = null;
                        }
                        List<string> filter = new List<string>();
                        foreach (var item in joinCoumnId.Value)
                        {
                            filter.Add("{ " + join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELD].ToString() + ": \"" + item + "\" }");
                        }
                        string filterQuery = "{ $or: [ " + string.Join(",", filter) + "] }";

                        JArray joinCollectionData = DBProxy.Get(join[CommonConst.CommonField.DB_JOIN_DESTINATION_COLELCTION].ToString(), filterQuery, fields);
                        foreach (JObject joinData in joinCollectionData)
                        {
                            if (joinData[join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELD].ToString()] != null)
                            {
                                var joinid = joinData[join[CommonConst.CommonField.DB_JOIN_DESTINATION_FIELD].ToString()].ToString();

                                var dataJoin = (data[CommonConst.CommonField.DATA] as JArray).FirstOrDefault(f => f[join[CommonConst.CommonField.DB_JOIN_SOURCE_FIELD].ToString()].ToString() == joinid);
                                if (dataJoin != null)
                                {
                                    if (dataJoin[join[CommonConst.CommonField.DB_JOIN_VALUE].ToString()] == null)
                                    {
                                        dataJoin[join[CommonConst.CommonField.DB_JOIN_VALUE].ToString()] = new JArray();
                                    }
                                    (dataJoin[join[CommonConst.CommonField.DB_JOIN_VALUE].ToString()] as JArray).Add(joinData);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}