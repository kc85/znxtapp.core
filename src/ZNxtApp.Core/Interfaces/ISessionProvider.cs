using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface ISessionProvider
    {
        T GetValue<T>(string key);
        void ResetSession();
        void SetValue<T>(string key, T value);
    }
}
