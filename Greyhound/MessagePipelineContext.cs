namespace Greyhound
{
    internal class MessagePipelineContext<T> : IMessagePipelineContext<T>
    {
        public IMessage<T> Message { get; internal set; }

        public bool Cancel { get; set; }
    }
}