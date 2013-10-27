using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Greyhound
{
    internal static class Utils
    {
        private static readonly Lazy<MethodInfo> PutMessageMethod;
        private static readonly ConcurrentDictionary<Type, MethodInfo> CachedGenericMethods;

        static Utils()
        {
            CachedGenericMethods = new ConcurrentDictionary<Type, MethodInfo>();
            PutMessageMethod = new Lazy<MethodInfo>(() => typeof (GreyhoundBus).GetMethod("PutMessage"));
        }

        internal static void PutNonGenericMessageOnBus(IMessage message, GreyhoundBus greyhoundBus)
        {
            Type messageType = message.GetType();
            if (!messageType.IsGenericType)
                return; //Discard this?
            Type messageDataType = messageType.GetGenericArguments()[0];
            CachedGenericMethods.GetOrAdd(messageDataType, t => PutMessageMethod.Value.MakeGenericMethod(t))
                                 .Invoke(greyhoundBus, new object[] {message});
        }
    }
}