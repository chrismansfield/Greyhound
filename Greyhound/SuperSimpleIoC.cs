using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Greyhound
{
    internal class SuperSimpleIoC
    {
        private static SuperSimpleIoC _container;
        private Dictionary<Type, MethodInfo> _providers;
        private ConcurrentDictionary<Type, object> _singletonObjects;

        public SuperSimpleIoC()
        {
            _singletonObjects = new ConcurrentDictionary<Type, object>();
        }

        private Dictionary<Type, MethodInfo> Providers
        {
            get
            {
                if (_providers == null)
                    InitializeProviders();
                return _providers;
            }
        }

        public static void Initialize()
        {
            Initialize(new SuperSimpleIoC());
        }

        public static void Initialize(SuperSimpleIoC container)
        {
            _container = container;
        }

        public static T Get<T>()
        {
            if (_container == null)
                Initialize();
            return _container.GetInstance<T>();
        }

        private void InitializeProviders()
        {
            _providers = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(m => m.ReturnType);
        }

        private T GetInstance<T>()
        {
            if (!Providers.ContainsKey(typeof (T)))
                throw new TypeLoadException(string.Format("The type {0} is not in the current container", typeof (T)));

            return (T) Providers[typeof (T)].Invoke(_container, Enumerable.Empty<object>().ToArray());
        }

        public virtual ISubscriberCoordinator ProvideSubscriberManager()
        {
            return new SubscriberCoordinator(ProvideSubscriberRunner());
        }

        public virtual ISubscriberRunner ProvideSubscriberRunner()
        {
            return GetSingleton<ISubscriberRunner>(() => new SubscriberRunner());
        }

        private T GetSingleton<T>(Func<T> factory)
        {
            var singletonType = typeof(T);
            return (T) _singletonObjects.GetOrAdd(singletonType, t => factory());
        }
    }
}