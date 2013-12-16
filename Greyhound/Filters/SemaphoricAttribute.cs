using System;
using System.Threading;

namespace Greyhound.Filters
{
    /// <summary>
    /// Applies a runnable filter to the subscriber, that limits the number of concurrent calls that can be made to the subscriber
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SemaphoricAttribute : RunnableAttribute
    {
        private readonly SemaphoreSlim _semaphore;
        
        public SemaphoricAttribute(int maxCount)
        {
            _semaphore = new SemaphoreSlim(maxCount);
        }

        public override bool Evaluate(IMessage<object> message)
        {
            if (_semaphore.CurrentCount > 0)
            {
                lock (_semaphore)
                {
                    if (_semaphore.CurrentCount > 0)
                    {
                        _semaphore.Wait();
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Expire(IMessageContext<object> message)
        {
            _semaphore.Release();
        }
    }
}