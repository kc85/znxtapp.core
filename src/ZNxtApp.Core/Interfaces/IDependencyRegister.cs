using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZNxtApp.Core.Interfaces
{
    public interface IDependencyRegister : IDisposable
    {
        void Register<TF, TT>() where TT : TF;

        void Register<TF, TT>(string key) where TT : TF;

        void RegisterInstance<TF>(TF obj);

        void RegisterInstance<TF>(TF obj, string name);

        void Register(Type typeSource, Type typeDestination);

        IDependencyResolver GetResolver();
    }
}
