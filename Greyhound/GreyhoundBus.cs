namespace Greyhound
{
    public sealed class GreyhoundBus
    {
        internal readonly GreyhoundBus ErrorBus;
        internal readonly bool IsErrorBus;
        private readonly ISubscriberCoordinator _subscriberCoordinator;
        private readonly ISubscriberRunner _subscriberRunner;

        public GreyhoundBus()
            :this(false)
        {
        }

        private GreyhoundBus(bool isErrorBus)
        {
            IsErrorBus = isErrorBus;
            _subscriberCoordinator = SuperSimpleIoC.Get<ISubscriberCoordinator>();
            _subscriberRunner = SuperSimpleIoC.Get<ISubscriberRunner>();
            if (!isErrorBus)
            {
                ErrorBus = new GreyhoundBus(true);
            }

            _subscriberRunner.Start();
        }
        
        public void PutMessage<T>(IMessage<T> message)
        {
            _subscriberCoordinator.PutMessageToSubscribers(Message.Context(message, this));
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            if (!IsErrorBus && subscriber is ErrorSubscriber<T>)
                ErrorBus.AddSubscriber(subscriber);
            else
                _subscriberCoordinator.AddSubscriber(subscriber);
        }
    }
}