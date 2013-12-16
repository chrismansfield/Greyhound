using System;
using System.Collections.Generic;

namespace Greyhound
{
    public interface ISubscriber<in T>
    {
        void OnMessage(IMessageContext<T> messageContext);
    }
}