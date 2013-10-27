using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Greyhound
{
    public class InMemoryPersistor : Persistor
    {
        private readonly ConcurrentDictionary<object, object> _messages = new ConcurrentDictionary<object, object>();

        public override void Persist(object key, object message)
        {
            _messages.AddOrUpdate(key, message, (_, __) => message);
        }

        public override void Delete(object key)
        {
            object stub;
            _messages.TryRemove(key, out stub);
        }

        public override IEnumerable<IMessage> Restore(string busName)
        {
            return _messages.Values.OfType<IMessage>();
        }
    }
}