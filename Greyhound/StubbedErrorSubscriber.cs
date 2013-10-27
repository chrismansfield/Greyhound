namespace Greyhound
{
    internal class StubbedErrorSubscriber<T> : ErrorSubscriber<T>
    {
        protected override void OnError(MessageContext<T> messageContext, ErrorMessage<T> message)
        {
        }
    }
}