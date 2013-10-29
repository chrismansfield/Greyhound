using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Greyhound
{
    public class InMemoryPersistor : IPersistor
    {
        private static readonly ConcurrentDictionary<MessageKey, IMessage<object>> Messages;

        static InMemoryPersistor()
        {
            Messages = new ConcurrentDictionary<MessageKey, IMessage<object>>();
        }

        public void Persist(string busName, Guid key, IMessage<object> message)
        {
            Messages.AddOrUpdate(new MessageKey(busName, key), message, (_, __) => message);
        }

        public void Delete(Guid key)
        {
            IMessage<object> stub;
            Messages.TryRemove(new MessageKey(MessageKey.AnyBusName, key), out stub);
        }

        public IEnumerable<IMessage<object>> Restore(string busName)
        {
            return Messages.Where(x => x.Key.BusName == busName).Select(x => x.Value).ToArray();
        }

        public static IDictionary<MessageKey, IMessage<object>> GetAllCurrentlyPersistedMessages()
        {
            return new Dictionary<MessageKey, IMessage<object>>(Messages);
        }

        
    }
}