using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Greyhound
{
    public class MessageContext<T>
    {
        private readonly ConcurrentBag<IMessage> _errorMessages;
        private readonly ConcurrentBag<IMessage> _messages;
        private readonly ConcurrentBag<Action<GreyhoundBus>> _pendingPutMessages;

        public MessageContext(IMessage<T> message)
        {
            Message = message;
            _messages = new ConcurrentBag<IMessage>();
            _errorMessages = new ConcurrentBag<IMessage>();
            _pendingPutMessages = new ConcurrentBag<Action<GreyhoundBus>>();
        }

        public IMessage<T> Message { get; private set; }

        public void AddEvent<TEvent>(IMessage<TEvent> newMessage)
        {
            _pendingPutMessages.Add(gb => gb.PutMessage(newMessage));
        }

        internal void AddError(string subscriberName, IMessage<T> message, Exception exception)
        {
            var errorMessage = Greyhound.Message.Error(message, subscriberName, exception);
            _pendingPutMessages.Add(gb =>
            {
                if (!gb.IsErrorBus)
                    gb.ErrorBus.PutMessage(errorMessage);
            });
        }

        public void PutAllEventsOnBus(GreyhoundBus greyhoundBus)
        {
            Action<GreyhoundBus> action;
            while (_pendingPutMessages.TryTake(out action))
                action(greyhoundBus);
        }
    }
}