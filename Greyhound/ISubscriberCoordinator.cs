using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greyhound
{
    internal interface ISubscriberCoordinator
    {
        ICollection<ISubscriberManager> SubscriberManagers { get; }
        void PutMessageToSubscribers<T>(MessageContext<T> messageContext);
        void AddSubscriber<T>(ISubscriber<T> subscriber);
    }
}