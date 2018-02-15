using Newtonsoft.Json.Linq;
using System.IO;

namespace ZNxtAap.Core.Helpers
{
    public static class JObjectHelper
    {
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
    }
}