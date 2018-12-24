using System;
using ZNxtApp.Core.Consts;
using ZNxtApp.Core.Interfaces;
using ZNxtApp.Core.Model;

namespace ZNxtApp.Core.Web.Helper
{
    public static class AuthTokenHelper
    {
        internal static bool IsAuthTokenExits(IHttpContextProxy httpProxy)
        {
            return !string.IsNullOrEmpty(httpProxy.GetHeader(CommonConst.CommonField.AUTH_TOKEN));
        }

        internal static string GetUserId(IHttpContextProxy httpProxy, IDBService dbProxy, IEncryption encryption)
        {
            var authtoken = httpProxy.GetHeader(CommonConst.CommonField.AUTH_TOKEN);
            DBQuery query = new DBQuery()
            {
                Filters = new FilterQuery() {
                new Filter(CommonConst.CommonField.AUTH_TOKEN, encryption.Encrypt(authtoken)) }
            };
            var data = dbProxy.Get(CommonConst.Collection.AUTH_TOKEN_COLLECTION, query);
            if (data.Count != 1)
            {
                throw new Exception("Invalid  auth token");
            }
            return data[0][CommonConst.CommonField.USER_ID].ToString();
        }
    }
}