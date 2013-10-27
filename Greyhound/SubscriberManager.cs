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

        public Task<MessageContext<T>> PutMessageToSubscribers<T>(IMessage<T> message)
        {
            return Task.Run(async () =>
                {
                    var messageContext = new MessageContext<T>(message);
                    IEnumerable<ISubscriber<T>> subscribers = _subscribers.OfType<ISubscriber<T>>()
                                                                          .WhereFiltersMatch(message);
                    await Task.WhenAll(subscribers.Select(subscriber => InvokeSubscriber(subscriber, messageContext)));

                    return messageContext;
                });
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public bool HasErrorSubscriber<T>()
        {
            return _subscribers.OfType<ErrorSubscriber<T>>().Any();
        }

        private Task InvokeSubscriber<T>(ISubscriber<T> subscriber, MessageContext<T> messageContext)
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
    }
}