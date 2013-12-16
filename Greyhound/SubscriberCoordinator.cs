using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Greyhound
{
    internal class SubscriberCoordinator : ISubscriberCoordinator
    {
        private readonly ICollection<ISubscriberManager> _subscriberManagers;

        public SubscriberCoordinator(ISubscriberRunner subscriberRunner)
        {
            _subscriberManagers = new Collection<ISubscriberManager>();
            subscriberRunner.RegisterCoordinator(this);
        }

        public ICollection<ISubscriberManager> SubscriberManagers
        {
            get { return _subscriberManagers; }
        }

        public void PutMessageToSubscribers<T>(MessageContext<T> messageContext)
        {
            var subscriberManagers = SubscriberManagers.OfType<ISubscriberManager<T>>();
            foreach (var subscriberManager in subscriberManagers)
            {
                subscriberManager.PutMessage(messageContext);
            }
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            SubscriberManagers.Add(new SubscriberManager<T>(subscriber));
        }
    }
}

