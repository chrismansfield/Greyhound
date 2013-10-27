using System;
using System.Threading.Tasks;

namespace Greyhound
{
    public class GreyhoundBus
    {
        internal readonly GreyhoundBus ErrorBus;
        private readonly bool _isErrorBus;
        private readonly ISubscriberManager _subscriberManager;
        private Pipeline _pipline;

        public GreyhoundBus()
            : this(Guid.NewGuid().ToString())
        {
        }

        public GreyhoundBus(string name)
            : this(name, false)
        {
        }

        internal GreyhoundBus(string name, bool isErrorBus)
        {
            Name = name;
            _isErrorBus = isErrorBus;
            _subscriberManager = SuperSimpleIoC.Get<ISubscriberManager>();
            if (!isErrorBus)
            {
                ErrorBus = new GreyhoundBus(string.Format("{0}.{1}", name, "error"), isErrorBus: true)
                    {
                        Pipline = {Persistor = Pipline.Persistor}
                    };
            }
        }

        public string Name { get; private set; }

        public Pipeline Pipline
        {
            get
            {
                if (_pipline == null)
                    _pipline = new Pipeline();
                return _pipline;
            }
            set { _pipline = value; }
        }

        public void RestoreBus()
        {
            foreach (IMessage message in Pipline.Persistor.Restore(Name))
                Utils.PutNonGenericMessageOnBus(message, this);
            if(!_isErrorBus)
                ErrorBus.RestoreBus();
        }

        public void PutMessage<T>(IMessage<T> message)
        {
            MessagePipelineContext<T> processedMessage = Pipline.ProcessInboundMessage(message);

            _subscriberManager.PutMessageToSubscribers(processedMessage.Message)
                              .ContinueWith(OnMessageDone);
        }

        private void OnMessageDone<T>(Task<MessageContext<T>> context)
        {
            MessageContext<T> messageContext = context.Result;
            messageContext.PutAllEventsOnBus(this);
            Pipline.ProcessExpiringMessage(messageContext.Message);
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            if (!_isErrorBus && subscriber is ErrorSubscriber<T>)
                ErrorBus.AddSubscriber(subscriber);
            else
                _subscriberManager.AddSubscriber(subscriber);
        }
    }
}
