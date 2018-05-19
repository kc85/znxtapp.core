using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Model
{
    public class UserModel
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string email  { get; set; }
        public string name { get; set; }
        public string user_type { get; set; }
        public string email_validation_required { get; set; }
        public string phone_validation_required { get; set; }
        public List<string> groups { get; set; }
        public UserModel()
        {
            groups = new List<string>();
        }
    }
}
