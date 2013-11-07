using System;
using System.Threading.Tasks;

namespace Greyhound
{
    public sealed class GreyhoundBus
    {
        internal readonly GreyhoundBus ErrorBus;
        internal readonly bool IsErrorBus;
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

        private GreyhoundBus(string name, bool isErrorBus)
        {
            Name = name;
            IsErrorBus = isErrorBus;
            _subscriberManager = SuperSimpleIoC.Get<ISubscriberManager>();
            if (!isErrorBus)
            {
                ErrorBus = new GreyhoundBus(string.Format("{0}.{1}", name, "error"), true)
                {
                    Pipeline = {Persistor = Pipeline.Persistor}
                };
            }
        }

        public string Name { get; private set; }

        public Pipeline Pipeline
        {
            get
            {
                if (_pipline == null)
                    _pipline = new Pipeline(this);
                return _pipline;
            }
        }

        public void Restore()
        {
            foreach (var message in Pipeline.Persistor.Restore(Name))
                Utils.PutMessageTypeSafe(message, this);
            if (!IsErrorBus)
                ErrorBus.Restore();
        }

        public void PutMessage<T>(IMessage<T> message)
        {
            IMessagePipelineContext<T> processedMessage = Pipeline.ProcessInboundMessage(message);

            if (processedMessage.Cancel)
                return;

            _subscriberManager.PutMessageToSubscribers(Message.Context(processedMessage.Message, this))
                .ContinueWith(OnMessageDone, TaskContinuationOptions.NotOnFaulted);
        }

        private void OnMessageDone<T>(Task<MessageContext<T>> context)
        {
            MessageContext<T> messageContext = context.Result;
            Pipeline.ProcessExpiringMessage(messageContext.Message);
        }

        public void AddSubscriber<T>(ISubscriber<T> subscriber)
        {
            if (!IsErrorBus && subscriber is ErrorSubscriber<T>)
                ErrorBus.AddSubscriber(subscriber);
            else
                _subscriberManager.AddSubscriber(subscriber);
        }
    }
}