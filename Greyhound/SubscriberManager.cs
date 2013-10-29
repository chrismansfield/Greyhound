using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Greyhound
{
    internal class SubscriberManager : ISubscriberManager
    {
        private readonly ICollection<object> _subscribers;

        public SubscriberManager()
        {
            _subscribers = new Collection<object>();
        }

        public Task<MessageContext<T>> PutMessageToSubscribers<T>(MessageContext<T> messageContext)
        {
            return Task.Run(async () =>
            {
                IEnumerable<ISubscriber<T>> subscribers = _subscribers.OfType<ISubscriber<T>>()
                    .WhereFiltersMatch(messageContext.Message);
                await Task.WhenAll(subscribers.Select(subscriber => InvokeSubscriber(subscriber, messageContext)));

                return messageContext;
            });
        }
        
        private static Task InvokeSubscriber<T>(ISubscriber<T> subscriber, MessageContext<T> messageContext)
        {
            return Task.Run(() =>
            {
                try
                {
                    subscriber.OnMessage(messageContext);
                }
                catch (Exception e)
                {
                    messageContext.AddError(subscriber.GetType().Name, messageContext.Message, e);
                }
            });
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            _subscribers.Add(subscriber);
        }

        
    }
}