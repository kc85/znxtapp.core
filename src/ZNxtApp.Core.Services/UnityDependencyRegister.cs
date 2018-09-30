using System;
using Unity;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Services
{
    public class UnityDependencyRegister : IDependencyRegister
    {
        private UnityContainer _unityContainer;

        private IDependencyResolver _dependencyResolver;

        public UnityDependencyRegister()
        {
            _unityContainer = new UnityContainer();
            _dependencyResolver = new UnityDependencyResolver(_unityContainer);
        }

        public void Register<TF, TT>() where TT : TF
        {
            _unityContainer.RegisterType<TF, TT>();
        }

        public void Register<TF, TT>(string key) where TT : TF
        {
            _unityContainer.RegisterType<TF, TT>(key);
        }

        public void RegisterInstance<TF>(TF obj)
        {
            _unityContainer.RegisterInstance<TF>(obj);
        }

        public void RegisterInstance<TF>(TF obj, string name)
        {
            _unityContainer.RegisterInstance<TF>(name, obj);
        }

        public void Register(Type typeSource, Type typeDestination)
        {
            _unityContainer.RegisterType(typeSource, typeDestination);
        }

        public IDependencyResolver GetResolver()
        {
            return _dependencyResolver;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _unityContainer != null)
            {
                _unityContainer.Dispose();
            }
        }
    }
}