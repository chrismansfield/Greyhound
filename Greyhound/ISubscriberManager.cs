namespace Greyhound
{
    internal interface ISubscriberManager
    {
        bool CanRunNext();
        void RunNext();
    }

    internal interface ISubscriberManager<in TMessage> : ISubscriberManager
    {
        void PutMessage(IMessageContext<TMessage> message);
    }
}