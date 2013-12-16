using System.Threading;

namespace Greyhound.Filters
{
    /// <summary>
    /// Synchronizes access to a subscriber, ensuring that the subscriber is only processing one message at a time.
    /// This attribute is the same as using the <see cref="SemaphoricAttribute" /> with the value of 1, however this
    /// attribute uses a much more efficient way of blocking.
    /// </summary>
    public sealed class SynchronizedAttribute : RunnableAttribute
    {
        private readonly object _lock = new object();

        public override bool Evaluate(IMessage<object> message)
        {
            return Monitor.TryEnter(_lock);
        }

        public override void Expire(IMessageContext<object> message)
        {
            Monitor.Exit(_lock);
        }
    }
}