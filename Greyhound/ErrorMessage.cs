using System;

namespace Greyhound
{
    public class ErrorMessage<T> : Message<T>
    {
        public string SubscriberName { get; internal set; }

        public Exception Exception { get; internal set; }
    }
}