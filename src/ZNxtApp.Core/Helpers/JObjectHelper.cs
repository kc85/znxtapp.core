using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using ZNxtApp.Core.Consts;

namespace ZNxtApp.Core.Helpers
{
    public static class JObjectHelper
    {
        public static T Deserialize<T>(JObject data)
        {
            return Deserialize<T>(data.ToString());
        }
        public static T Deserialize<T>(string data)
        {
           return JsonConvert.DeserializeObject<T>(data);
        }
        public static JObject Serialize<T>(T data)
        {
            string varData = JsonConvert.SerializeObject(data);
            return JObject.Parse(varData);
        }

        public static JArray GetJArrayFromFile(string filePath)
        {
            JArray arrData = new JArray();
            var filedata = File.ReadAllText(filePath);
            arrData = JArray.Parse(filedata);
            return arrData;
        }

        public static JObject GetJObjectFromFile(string filePath)
        {
            JObject jobjData = new JObject();
            var filedata = File.ReadAllText(filePath);
            jobjData = JObject.Parse(filedata);
            return jobjData;
        }

        public static void WriteJSONData(string filePath, JToken data)
        {
            File.WriteAllText(filePath, data.ToString());
        }

        public static  JObject Marge(JObject obj1, JObject obj2, MergeArrayHandling mergeType)
        {
            obj1.Merge(obj2, new JsonMergeSettings
            {
                MergeArrayHandling = mergeType
            });
            return obj1;
        }
        public static bool TryParseJson(string data, ref JObject jsonObj)
        {
            try
            {
                jsonObj = JObject.Parse(data);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public static JObject GetJObjectDbDataFromFile(FileInfo fi, string contentType, string basepath, string moduleName,string pathPrefix = "")
        {
            string fileData = GetData(fi.FullName, contentType);
            string wwwwpath = fi.FullName.Replace(basepath, "").Replace("\\", "/");
            JObject data = new JObject();
            data[CommonConst.CommonField.DISPLAY_ID] = CommonUtility.GetNewID();
            data[CommonConst.CommonField.FILE_PATH] = string.Format("{0}{1}", pathPrefix,wwwwpath);
            data[CommonConst.CommonField.DATA] = fileData;
            data[CommonConst.CommonField.CREATED_DATA_DATE_TIME] = DateTime.Now;
            data[CommonConst.CommonField.FILE_SIZE] = fi.Length;
            data[CommonConst.CommonField.MODULE_NAME] = moduleName;
            data[CommonConst.CommonField.CONTENT_TYPE] = contentType;
            data[CommonConst.CommonField.ÌS_OVERRIDE] = false;
            data[CommonConst.CommonField.OVERRIDE_BY] = CommonConst.CommonValue.NONE;
            return data;
        }

        private static string GetData(string path, string contentType)
        {
            if (CommonUtility.IsTextConent(contentType))
            {
                return GetTextData(path);
            }
            else
            {
                return GetBinaryData(path);
            }
        }

        private static string GetBinaryData(string path)
        {
            byte[] fileData = File.ReadAllBytes(path);
            return CommonUtility.GetBase64(fileData);
        }

        private static string GetTextData(string path)
        {
            return File.ReadAllText(path);
        }

    }
}