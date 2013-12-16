using System.Collections.Concurrent;

namespace Greyhound
{
    internal interface ISubscriberRunner
    {
        void RegisterCoordinator(ISubscriberCoordinator coordinator);

        void UnregisterCoordinator(ISubscriberCoordinator coordinator);

        void Start();

        void Stop();
    }
}