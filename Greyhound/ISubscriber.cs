using System.Collections.Generic;

namespace Greyhound
{
    public interface ISubscriber<T>
    {
        IEnumerable<IFilter<T>> GetFilters();

        void OnMessage(MessageContext<T> messageContext);
    }
}