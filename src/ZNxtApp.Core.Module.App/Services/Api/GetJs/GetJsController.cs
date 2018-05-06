using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZNxtApp.Core.Config;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Model;
using ZNxtApp.Core.Services;

namespace ZNxtApp.Core.Module.App.Services.Api.GetJs
{
    public class GetJsController : ViewBaseService
    {
        public GetJsController(ParamContainer paramContainer) : base(paramContainer)
        {
        }

        public byte [] Get()
        {
            try
            {
                var path = HttpProxy.GetQueryString("path");
                var filterQuery = "{" + CommonConst.CommonField.FILE_PATH + ":/.js$/i}";
                DBProxy.Collection = CommonConst.Collection.STATIC_CONTECT;
                var data = DBProxy.Get(filterQuery,new List<string> { CommonConst.CommonField.FILE_PATH });
                var listOfArrays = new List<byte[]>();
                var queryRecords = data.Select(l => new
                {
                    length = l[CommonConst.CommonField.FILE_PATH].ToString().Length,
                    file_path = l[CommonConst.CommonField.FILE_PATH].ToString()

                }).OrderBy(o => o.length).ToList();

                foreach (var item in queryRecords)
                {
                    if (item.file_path.IndexOf(path) == 0)
                    {
                        string jspath = item.file_path;
                        if (jspath.IndexOf(CommonConst.CommonValue.APP_BACKEND_FOLDERPATH) == 1)
                        {
                            jspath = jspath.Replace(string.Format("/{0}", CommonConst.CommonValue.APP_BACKEND_FOLDERPATH), ApplicationConfig.AppBackendPath);
                        }
                        else if (jspath.IndexOf(CommonConst.CommonValue.APP_FRONTEND_FOLDERPATH) == 1)
                        {
                            jspath = jspath.Replace(string.Format("/{0}", CommonConst.CommonValue.APP_FRONTEND_FOLDERPATH), "/");
                        }
                        var content = ContentHandler.GetContent(jspath);
                        if (content != null)
                        {
                            listOfArrays.Add(content);
                            listOfArrays.Add(Encoding.UTF8.GetBytes("\n"));
                        }
                    }
                }

                return listOfArrays
                   .SelectMany(a => a)
                   .ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error in GetJs {0}", ex.Message), ex);
                throw;
            }
            
        }
    }
}
