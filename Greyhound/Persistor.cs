using System.Collections.Generic;

namespace Greyhound
{
    public abstract class Persistor : IPersistor
    {
        public object GetKey(object data)
        {
            return KeyManager.GetKey(data);
        }

        public abstract void Persist(object key, object message);
        public abstract void Delete(object key);

        public abstract IEnumerable<IMessage> Restore(string busName);
    }
}