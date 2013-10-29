namespace Greyhound
{
    public interface IMessageProcessor<T>
    {
        void ProcessMessage(IMessagePipelineContext<T> context);
    }
}