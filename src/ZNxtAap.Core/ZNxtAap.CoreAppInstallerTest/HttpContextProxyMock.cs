using System;
using ZNxtAap.Core.Interfaces;

namespace ZNxtAap.CoreAppInstallerTest
{
    public class HttpContextProxyMock : IHttpContextProxy
    {
        public string GetURIAbsolutePath()
        {
            throw new NotImplementedException();
        }

        public string GetHttpMethod()
        {
            throw new NotImplementedException();
        }

        public int ResponseStatusCode
        {
            get { throw new NotImplementedException(); }
        }

        public string ResponseStatusMessage
        {
            get { throw new NotImplementedException(); }
        }

        public byte[] Response
        {
            get { throw new NotImplementedException(); }
        }

        public void SetResponse(int statusCode, Newtonsoft.Json.Linq.JObject data)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(int statusCode, string data = null)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(int statusCode, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(string data)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(byte[] data)
        {
            throw new NotImplementedException();
        }

        public string ContentType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string GetMimeType(string fileName)
        {
            throw new NotImplementedException();
        }

        public string GetRequestBody()
        {
            throw new NotImplementedException();
        }

        public T GetRequestBody<T>()
        {
            throw new NotImplementedException();
        }

        public string GetQueryString(string key)
        {
            throw new NotImplementedException();
        }

        public string GetFormData(string key)
        {
            throw new NotImplementedException();
        }
    }
}