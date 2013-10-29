using System.Collections.Generic;
using System.Linq;

namespace Greyhound
{
    public abstract class ErrorSubscriber<T> : ISubscriber<T>
    {
        public IEnumerable<IFilter<T>> GetFilters()
        {
            return Enumerable.Empty<IFilter<T>>();
        }

        public void OnMessage(IMessageContext<T> messageContext)
        {
            OnError((ErrorMessage<T>) messageContext.Message);
        }

        protected abstract void OnError(ErrorMessage<T> message);
    }
}