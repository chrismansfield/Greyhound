namespace Greyhound
{
    public interface IMessageProcessor<in T>
    {
        void ProcessMessage(IMessagePipelineContext<T> context);
    }
}