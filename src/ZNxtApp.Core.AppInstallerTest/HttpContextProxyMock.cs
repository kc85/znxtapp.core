﻿using System;
using System.Collections.Generic;
using System.IO;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core
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

        public string GetContentType(string path)
        {
            throw new NotImplementedException();
        }

        public string GetContentType(FileInfo pathInfo)
        {
            throw new NotImplementedException();
        }

        public void UnloadAppDomain()
        {
            throw new NotImplementedException();
        }

        public string GetHeader(string key)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetHeaders()
        {
            throw new NotImplementedException();
        }

        public T GetTempValue<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void SetTempValue<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public DateTime InitDateTime
        {
            get { throw new NotImplementedException(); }
        }

        public string TransactionId
        {
            get { throw new NotImplementedException(); }
        }

        public string SessionID
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<string, string> ResponseHeaders
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
    }
}