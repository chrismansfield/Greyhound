namespace Greyhound
{
    public interface IMessage
    {
        bool IsEvent { get; set; }
    }

    public interface IMessage<out T> : IMessage
    {
        T Data { get; }
    }

    public class Message<T> : IMessage<T>
    {
        public bool IsEvent { get; set; }

        public T Data { get; internal set; }
    }
}