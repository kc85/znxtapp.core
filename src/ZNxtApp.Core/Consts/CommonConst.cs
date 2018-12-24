using System;
using System.Collections.Generic;
using System.Linq;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Consts
{
    public static partial class CommonConst
    {
        public const string CONTENT_TYPE_APPLICATION_JSON = "application/json";
        public const string CONTENT_TYPE_TEXT_HTML = "text/html";
        public const string CONTENT_TYPE_TEXT_JS = "text/javascript";
        public const string CONTENT_TYPE_TEXT_CSS = "text/css";
        public const string ENVIRONMENT_SETTING_KEY = "Environment";
        public const string CONFIG_FILE_EXTENSION = ".json";
        public const string EMPTY_JSON_OBJECT = "{}";
        public const string MODULE_INSTALL_WWWROOT_FOLDER = "wwwroot";
        public const string MODULE_INSTALL_DLLS_FOLDER = "dlls";
        public const string MODULE_INSTALL_COLLECTIONS_FOLDER = "collections";
        public const int _404_RESOURCE_NOT_FOUND = 404;
        public const int _200_OK = 200;
        public const int _1_SUCCESS = 1;
        public const int _400_BAD_REQUEST = 400;
        public const int _401_UNAUTHORIZED = 401;
        public const int _500_SERVER_ERROR = 500;
        private const string UNKNOWN_MESSAGE = "UNKNOWN_STATUS_CODE";

        public static MessageText Messages
        {
            get
            {
                return MessageText.GetMessage();
            }
        }

        public class MessageText
        {
            private Dictionary<int, string> text = new Dictionary<int, string>();
            private static MessageText _messageText;
            private static object lockObj = new object();

            public string this[int value]
            {
                get
                {
                    if (text.ContainsKey(value))
                    {
                        return text[value];
                    }
                    else
                    {
                        var result = GetMessage(value);
                        if (!string.IsNullOrEmpty(result))
                        {
                            text[value] = result;
                            return result;
                        }
                        else
                        {
                            return UNKNOWN_MESSAGE;
                        }
                    }
                }
            }

            private string GetMessage(int code)
            {
                foreach (var assemble in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type messageType in assemble.GetTypes()
               .Where(mytype => mytype.GetInterfaces().Contains(typeof(IMessageCodeContainer))))
                    {
                        dynamic obj = Activator.CreateInstance(messageType);
                        string text = obj.Get(code);
                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }
                }

                return string.Empty;
            }

            private MessageText()
            {
                text[CommonConst._200_OK] = "OK";
                text[CommonConst._404_RESOURCE_NOT_FOUND] = "NOT_FOUND";
                text[CommonConst._400_BAD_REQUEST] = "BAD_REQUEST";
                text[CommonConst._500_SERVER_ERROR] = "SERVER_ERROR";
                text[CommonConst._1_SUCCESS] = "SUCCESS";
                text[_401_UNAUTHORIZED] = "UNAUTHORIZED";
            }

            public static MessageText GetMessage()
            {
                if (_messageText == null)
                {
                    lock (lockObj)
                    {
                        _messageText = new MessageText();
                    }
                }
                return _messageText;
            }
        }

        public static class ActionMethods
        {
            public const string GET = "GET";
            public const string POST = "POST";
            public const string PUT = "PUT";
            public const string DELETE = "DELETE";
            public const string ACTION = "ACTION";
        }
    }
}