namespace Greyhound
{
    public interface IMessageContext<out T>
    {
        IMessage<T> Message { get; }
        void AddEvent<TEvent>(IMessage<TEvent> newMessage);
    }
}