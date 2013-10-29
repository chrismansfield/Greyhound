using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Greyhound
{
    public sealed class Pipeline
    {
        private readonly GreyhoundBus _greyhoundBus;
        private readonly ICollection<object> _inboundMessageProcessors;
        private IPersistor _persistor;

        public Pipeline(GreyhoundBus greyhoundBus)
        {
            _greyhoundBus = greyhoundBus;
            _inboundMessageProcessors = new Collection<object>();
        }

        public IPersistor Persistor
        {
            get
            {
                if (_persistor == null)
                    _persistor = new InMemoryPersistor();
                return _persistor;
            }
            set 
            { 
                _persistor = value;
                if(!_greyhoundBus.IsErrorBus)
                    _greyhoundBus.ErrorBus.Pipeline.Persistor = value;
            }
        }

        public void AddMessageProcessor<T>(IMessageProcessor<T> messageProcessor)
        {
            _inboundMessageProcessors.Add(messageProcessor);
        }

        internal IMessagePipelineContext<T> ProcessInboundMessage<T>(IMessage<T> message)
        {
            IMessagePipelineContext<T> context = RunProcessors(message, _inboundMessageProcessors);
            if (!context.Cancel)
                Persistor.Persist(_greyhoundBus.Name, context.Message.Id, (IMessage<object>) context.Message);
            return context;
        }

        private IMessagePipelineContext<T> RunProcessors<T>(IMessage<T> message, IEnumerable<object> messageProcessors)
        {
            var context = new MessagePipelineContext<T> {Message = message};
            foreach (var processor in messageProcessors.OfType<IMessageProcessor<T>>())
            {
                processor.ProcessMessage(context);
                if (context.Cancel)
                    return context;
            }
            return context;
        }

        internal void ProcessExpiringMessage<T>(IMessage<T> message)
        {
            Persistor.Delete(message.Id);
        }
    }
}
