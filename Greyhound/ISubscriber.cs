using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greyhound
{
    public interface ISubscriber<in T>
    {
        IEnumerable<IFilter<T>> GetFilters();

        void OnMessage(IMessageContext<T> messageContext);
    }

    public abstract class AsyncSubscriber<T> : ISubscriber<T>
    {
        public abstract IEnumerable<IFilter<T>> GetFilters();

        public void OnMessage(IMessageContext<T> messageContext)
        {}

        public abstract Task OnMessageAsync(IMessageContext<T> messageContext);
    }
}