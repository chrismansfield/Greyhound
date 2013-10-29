namespace Greyhound
{
    public interface IMessageContext<out T>
    {
        IMessage<T> Message { get; }
        void PutEvent<TEvent>(IMessage<TEvent> newMessage);
    }
}