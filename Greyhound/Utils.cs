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

        /// <summary>
        ///     Invokes PutMessage on the bus, using the concrete type of the message.
        ///     For instance, if a <see cref="IMessage{String}" /> is passed but boxed
        ///     in a <see cref="IMessage{Object}" /> this method ensures that
        ///     <see cref="GreyhoundBus.PutMessage{String}" /> is called
        /// </summary>
        internal static void PutMessageTypeSafe(IMessage<object> message, GreyhoundBus greyhoundBus)
        {
            Type messageType = message.GetType();
            Type messageDataType = messageType.GetGenericArguments()[0];
            CachedGenericMethods.GetOrAdd(messageDataType, t => PutMessageMethod.Value.MakeGenericMethod(t))
                .Invoke(greyhoundBus, new object[] {message});
        }
    }
}