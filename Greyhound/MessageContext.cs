using System;

namespace Greyhound
{
    internal class MessageContext<T> : IMessageContext<T>
    {
        private readonly GreyhoundBus _greyhoundBus;

        public MessageContext(IMessage<T> message, GreyhoundBus greyhoundBus)
        {
            _greyhoundBus = greyhoundBus;
            Message = message;
        }

        public IMessage<T> Message { get; private set; }

        public void AddEvent<TEvent>(IMessage<TEvent> newMessage)
        {
            _greyhoundBus.PutMessage(newMessage);
        }

        internal void AddError(string subscriberName, IMessage<T> message, Exception exception)
        {
            ErrorMessage<T> errorMessage = Greyhound.Message.Error(message, subscriberName, exception);
            if (!_greyhoundBus.IsErrorBus)
                _greyhoundBus.ErrorBus.PutMessage(errorMessage);
        }
    }
}