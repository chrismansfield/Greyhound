using System;

namespace Greyhound
{
    public interface IMessageContext<out T>
    {
        IMessage<T> Message { get; }
        void PutEvent<TEvent>(IMessage<TEvent> newMessage);
    }

    internal interface IInternalMessageContext<in T>
    {
        void AddError(string subscriberName, IMessage<T> message, Exception exception);
    }
}