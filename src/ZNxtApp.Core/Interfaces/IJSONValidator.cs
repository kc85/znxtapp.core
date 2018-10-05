using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;

namespace ZNxtApp.Core.Interfaces
{
    public interface IJSONValidator
    {
        bool Validate(JSchema schema, JToken data, out IList<string> messages);

        bool Validate(string schema, JToken data, out IList<string> messages);

        bool Validate(string schema, string data, out IList<string> messages);
    }
}