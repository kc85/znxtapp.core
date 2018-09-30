using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IDependencyResolver
    {
        T GetInstance<T>() where T : class;

        T GetInstance<T>(string key) where T : class;

        object GetInstance(Type type);
    }
}
