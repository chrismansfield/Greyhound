using System.Collections.Generic;

namespace Greyhound
{
    public interface ISubscriber<in T>
    {
        IEnumerable<IFilter<T>> GetFilters();

        void OnMessage(IMessageContext<T> messageContext);
    }
}