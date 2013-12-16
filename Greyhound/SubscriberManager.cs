using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Greyhound.Filters;

namespace Greyhound
{
    internal class SubscriberManager<TMessage> : ISubscriberManager<TMessage>
    {
        private readonly ISubscriber<TMessage> _subscriber;
        private readonly ConcurrentQueue<IMessageContext<TMessage>> _messages;
        private FilterAttribute[] _filters;
        private RunnableAttribute[] _runnableFilters;

        public SubscriberManager(ISubscriber<TMessage> subscriber)
        {
            _subscriber = ParseSubscriber(subscriber);
            _messages = new ConcurrentQueue<IMessageContext<TMessage>>();
        }

        private ISubscriber<TMessage> ParseSubscriber(ISubscriber<TMessage> subscriber)
        {
            var filters = subscriber.GetType().GetCustomAttributes<FilterAttribute>().ToArray();
            _runnableFilters = filters.OfType<RunnableAttribute>().ToArray();
            _filters = filters.Except(_runnableFilters).ToArray();

            return subscriber;
        }

        public void PutMessage(IMessageContext<TMessage> message)
        {
            _messages.Enqueue(message);
        }

        public bool CanRunNext()
        {
            bool result = false;
            IMessageContext<TMessage> messageContext;
            if (_messages.TryPeek(out messageContext))
                result = _runnableFilters.All(x => x.Evaluate((IMessage<object>)messageContext.Message));
            return result;
        }

        public void RunNext()
        {
            IMessageContext<TMessage> messageContext;
            if (_messages.TryDequeue(out messageContext) && ValidateMessage(messageContext.Message))
                InvokeSubscriber(_subscriber, messageContext);
            foreach (var runnableFilter in _runnableFilters)
                runnableFilter.Expire((IMessageContext<object>)messageContext);

        }

        private bool ValidateMessage(IMessage<TMessage> message)
        {
            return _filters.All(x => x.Evaluate((IMessage<object>)message));
        }

        private void InvokeSubscriber(ISubscriber<TMessage> subscriber, IMessageContext<TMessage> messageContext)
        {
            try
            {
                subscriber.OnMessage(messageContext);
            }
            catch (Exception e)
            {
                ((IInternalMessageContext<TMessage>) messageContext).AddError(subscriber.GetType().Name, messageContext.Message, e);
            }
        }
    }

}