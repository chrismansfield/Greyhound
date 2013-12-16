using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Greyhound
{
    internal class SubscriberRunner : ISubscriberRunner
    {
        private readonly ICollection<ISubscriberCoordinator> _subscriberCoordinators;
        private readonly object _lock;
        private readonly Timer _timer;

        public SubscriberRunner()
        {
            _lock = new object();
            _subscriberCoordinators = new List<ISubscriberCoordinator>();
            _timer = new Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void RegisterCoordinator(ISubscriberCoordinator coordinator)
        {
            _subscriberCoordinators.Add(coordinator);
        }

        public void UnregisterCoordinator(ISubscriberCoordinator coordinator)
        {
            if (_subscriberCoordinators.Contains(coordinator))
                lock (_lock)
                    if (_subscriberCoordinators.Contains(coordinator))
                        _subscriberCoordinators.Remove(coordinator);
        }

        public void Start()
        {
            lock (_lock)
            {
                _timer.Change(500, Timeout.Infinite);
            }
        }

        private void Restart()
        {
            lock (_lock)
            {
                _timer.Change(0, Timeout.Infinite);
            }

        }

        public void Stop()
        {
            lock (_lock)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void OnTick(object state)
        {
            int completionPortThreads;
            int workerThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            if (workerThreads > 0)
                ThreadPool.QueueUserWorkItem(RunItem);
            Restart();
        }

        private void RunItem(object state)
        {
            ISubscriberManager subscriberManager;
            lock (_lock)
            {
                subscriberManager = (from sc in _subscriberCoordinators
                                     from sm in sc.SubscriberManagers
                                     where sm.CanRunNext()
                                     select sm).FirstOrDefault();
            }
            if (subscriberManager != null)
                subscriberManager.RunNext();
        }
    }
}