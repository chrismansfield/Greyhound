namespace Greyhound
{
    public interface IMessagePipelineContext<out T>
    {
        IMessage<T> Message { get; }
        bool Cancel { get; set; }
    }
}