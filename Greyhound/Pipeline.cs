using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Greyhound
{
    public class Pipeline
    {
        protected ICollection<object> InboundMessageProcessors;
        private IPersistor _persistor;

        public Pipeline()
        {
            InboundMessageProcessors = new Collection<object>();
        }

        public virtual IPersistor Persistor
        {
            get
            {
                if (_persistor == null)
                    _persistor = new InMemoryPersistor();
                return _persistor;
            }
            set { _persistor = value; }
        }

        public void AddInboundMessageProcessor<T>(IMessageProcessor<T> messageProcessor)
        {
            InboundMessageProcessors.Add(messageProcessor);
        }

        public virtual MessagePipelineContext<T> ProcessInboundMessage<T>(IMessage<T> message)
        {
            MessagePipelineContext<T> context = RunProcessors(message, InboundMessageProcessors);
            if (!context.Cancel)
                Persistor.Persist(Persistor.GetKey(context.Message.Data), context.Message);
            return context;
        }

        private MessagePipelineContext<T> RunProcessors<T>(IMessage<T> message, IEnumerable<object> messageProcessors)
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

        public virtual void ProcessExpiringMessage<T>(IMessage<T> message)
        {
            Persistor.Delete(Persistor.GetKey(message.Data));
        }
    }
}