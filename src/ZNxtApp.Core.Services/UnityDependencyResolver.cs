using System;
using Unity;
using ZNxtApp.Core.Interfaces;

namespace ZNxtApp.Core.Services
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private UnityContainer _unityContainer;

        public UnityDependencyResolver(UnityContainer _container)
        {
            this._unityContainer = _container;
        }

        public T GetInstance<T>() where T : class
        {
            return _unityContainer.Resolve<T>();
        }

        public T GetInstance<T>(string key) where T : class
        {
            return _unityContainer.Resolve<T>(key);
        }

        public object GetInstance(Type type)
        {
            return _unityContainer.Resolve(type, string.Empty);
        }
    }
}