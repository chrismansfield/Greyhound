using System;

namespace Greyhound.Filters
{
    /// <summary>
    /// Applies a filter to the subscriber, than can evaluate whether the subscriber is ready to accept the current message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RunnableAttribute : FilterAttribute
    {
        public abstract void Expire(IMessageContext<object> message);
    }
}