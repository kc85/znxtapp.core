using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Services.Helper
{
    public class JSONValidator : IJSONValidator
    {
        public JSONValidator()
        {
        }

        public bool Validate(JSchema schema, JToken data, out IList<string> messages)
        {
            messages = new List<string>();
            return data.IsValid(schema, out messages);
        }

        public bool Validate(string schemaJson, JToken data, out IList<string> messages)
        {
            JSchema schema = JSchema.Parse(schemaJson);
            messages = new List<string>();
            return Validate(schema, data, out messages);
        }

        public bool Validate(string schemaJson, string data, out IList<string> messages)
        {
            JObject d = JObject.Parse(data);
            messages = new List<string>();
            return Validate(schemaJson, d, out messages);
        }
    }
}