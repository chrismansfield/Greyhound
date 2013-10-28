using System;

namespace Greyhound
{
    public static class Message
    {
        public static IMessage<T> Create<T>(T data)
        {
            return new Message<T> {Data = data};
        }

        public static ErrorMessage<T> Error<T>(IMessage<T> originalMessage, string subscriber, Exception exception)
        {
            return new ErrorMessage<T>
                {
                    IsEvent = originalMessage.IsEvent,
                    Data = originalMessage.Data,
                    SubscriberName = subscriber,
                    Exception = exception
                };
        }

        internal static MessageContext<T> Context<T>(IMessage<T> message)
        {
            return new MessageContext<T>(message);
        }
    }
}