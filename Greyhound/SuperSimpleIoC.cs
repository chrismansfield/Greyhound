using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Greyhound
{
    internal class SuperSimpleIoC
    {
        public static SuperSimpleIoC Container;
        private Dictionary<Type, MethodInfo> _providers;

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
            Container = container;
        }

        public static T Get<T>()
        {
            if (Container == null)
                Initialize();
            return Container.GetInstance<T>();
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

            return (T) Providers[typeof (T)].Invoke(Container, Enumerable.Empty<object>().ToArray());
        }

        public virtual ISubscriberManager ProvideSubscriberManager()
        {
            return new SubscriberManager();
        }
    }
}