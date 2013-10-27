namespace Greyhound
{
    public interface IMessageProcessor<T>
    {
        void ProcessMessage(MessagePipelineContext<T> context);
    }
}