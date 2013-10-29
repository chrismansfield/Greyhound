using System;

namespace Greyhound
{
    public static class Message
    {
        public static IMessage<T> Create<T>(T data)
        {
            return new Message<T>(Guid.NewGuid()) {Data = data};
        }

        public static ErrorMessage<T> Error<T>(IMessage<T> originalMessage, string subscriber, Exception exception)
        {
            return new ErrorMessage<T>(Guid.NewGuid())
            {
                Data = originalMessage.Data,
                SubscriberName = subscriber,
                Exception = exception
            };
        }

        internal static MessageContext<T> Context<T>(IMessage<T> message, GreyhoundBus greyhoundBus)
        {
            return new MessageContext<T>(message, greyhoundBus);
        }
    }

    internal class Message<T> : IMessage<T>
    {
        internal Message(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
        public T Data { get; internal set; }
    }
}