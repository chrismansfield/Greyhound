namespace Greyhound
{
    public class MessagePipelineContext<T>
    {
        public IMessage<T> Message { get; internal set; }

        public bool Cancel { get; set; }
    }
}