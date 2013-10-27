using System;
using System.Collections.Concurrent;

namespace Greyhound
{
    public class MessageContext<T>
    {
        private readonly ConcurrentBag<IMessage> _errorMessages;
        private readonly ConcurrentBag<IMessage> _messages;

        public MessageContext(IMessage<T> message)
        {
            Message = message;
            _messages = new ConcurrentBag<IMessage>();
            _errorMessages = new ConcurrentBag<IMessage>();
        }

        public IMessage<T> Message { get; private set; }

        public void AddEvent<TEvent>(IMessage<TEvent> newMessage)
        {
            _messages.Add(newMessage);
        }

        public void AddError(string name, IMessage<T> message, Exception exception)
        {
            _errorMessages.Add(Greyhound.Message.Error(message, name, exception));
        }

        public void PutAllEventsOnBus(GreyhoundBus greyhoundBus)
        {
            PutEventsOnQueue(_messages, greyhoundBus);
            PutEventsOnQueue(_errorMessages, greyhoundBus.ErrorBus);
        }

        private void PutEventsOnQueue(ConcurrentBag<IMessage> messages, GreyhoundBus greyhoundBus)
        {
            while (true)
            {
                IMessage message;
                if (!messages.TryTake(out message))
                    break;
                Utils.PutNonGenericMessageOnBus(message, greyhoundBus);
            }
        }
    }
}