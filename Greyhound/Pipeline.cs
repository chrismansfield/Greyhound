using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Greyhound
{
    public sealed class Pipeline
    {
        private readonly GreyhoundBus _greyhoundBus;
        private readonly ICollection<object> _inboundMessageProcessors;

        public Pipeline(GreyhoundBus greyhoundBus)
        {
            _greyhoundBus = greyhoundBus;
            _inboundMessageProcessors = new Collection<object>();
        }

        public void AddMessageProcessor<T>(IMessageProcessor<T> messageProcessor)
        {
            _inboundMessageProcessors.Add(messageProcessor);
        }

        internal IMessagePipelineContext<T> ProcessInboundMessage<T>(IMessage<T> message)
        {
            IMessagePipelineContext<T> context = RunProcessors(message, _inboundMessageProcessors);
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
    }
}
