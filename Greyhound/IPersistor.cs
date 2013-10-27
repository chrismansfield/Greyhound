using System.Collections.Generic;

namespace Greyhound
{
    public interface IPersistor
    {
        object GetKey(object data);

        void Persist(object key, object message);

        void Delete(object key);

        IEnumerable<IMessage> Restore(string busName);
    }
}