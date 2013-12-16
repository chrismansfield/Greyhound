using System;

namespace Greyhound.Filters
{
    /// <summary>
    /// Applies a filter to the subscriber, that can evaluate every message, and discard them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class FilterAttribute : Attribute
    {
        public abstract bool Evaluate(IMessage<object> message);
    }
}
