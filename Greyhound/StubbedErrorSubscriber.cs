namespace Greyhound
{
    internal class StubbedErrorSubscriber<T> : ErrorSubscriber<T>
    {
        protected override void OnError(IMessageContext<T> messageContext, ErrorMessage<T> message)
        {
        }
    }
}