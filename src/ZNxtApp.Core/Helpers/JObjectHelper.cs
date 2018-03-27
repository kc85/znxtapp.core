using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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
    }
}