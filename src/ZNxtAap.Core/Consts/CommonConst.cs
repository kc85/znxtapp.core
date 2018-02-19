using System.Collections.Generic;

namespace ZNxtAap.Core.Consts
{
    public static partial class CommonConst
    {
        public const string CONTENT_TYPE_APPLICATION_JSON = "application/json";
        public const string CONTENT_TYPE_TEXT_HTML = "text/html";
        public const string ENVIRONMENT_SETTING_KEY = "Environment";
        public const string CONFIG_FILE_EXTENSION = ".json";
        public const string EMPTY_JSON_OBJECT = "{}";
        public const string MODULE_INSTALL_WWWROOT_FOLDER  = "wwwroot";
        public const string MODULE_INSTALL_DLLS_FOLDER = "dlls";
        public const string MODULE_INSTALL_COLLECTIONS_FOLDER = "collections";
        public const int _404_RESOURCE_NOT_FOUND = 404;
        public const int _200_OK = 200;
        public const int _400_BAD_REQUEST = 400;
        

        public static MessageText Messages
        {
            get
            {
                return new MessageText();
            }
        }

        public class MessageText
        {
            private Dictionary<int, string> text = new Dictionary<int, string>();

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
                        return null;
                    }
                }
            }

            public MessageText()
            {
                text[CommonConst._200_OK] = "OK";
                text[CommonConst._404_RESOURCE_NOT_FOUND] = "NOT_FOUND";
                text[CommonConst._400_BAD_REQUEST] = "BAD_REQUEST";
            }
        }

    }
}