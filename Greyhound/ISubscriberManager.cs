using System.Threading.Tasks;

namespace Greyhound
{
    internal interface ISubscriberManager
    {
        Task<MessageContext<T>> PutMessageToSubscribers<T>(MessageContext<T> messageContext);
        void AddSubscriber<T>(ISubscriber<T> subscriber);
        bool HasErrorSubscriber<T>();
    }
}