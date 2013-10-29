using System;
using System.Collections.Generic;

namespace Greyhound
{
    public interface IPersistor
    {
        void Persist(string busName, Guid key, IMessage<object> message);

        void Delete(Guid key);

        IEnumerable<IMessage<object>> Restore(string busName);
    }
}