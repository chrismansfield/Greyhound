using System.Threading.Tasks;

namespace Greyhound
{
    internal interface ISubscriberManager
    {
        Task<MessageContext<T>> PutMessageToSubscribers<T>(IMessage<T> message);
        void AddSubscriber<T>(ISubscriber<T> subscriber);
        bool HasErrorSubscriber<T>();
    }
}