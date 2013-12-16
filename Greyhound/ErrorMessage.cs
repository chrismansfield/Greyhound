using System;

namespace Greyhound
{
    public class ErrorMessage<T> : IMessage<T>
    {
        internal ErrorMessage(Guid id)
        {
            Id = id;
        }

        public string SubscriberName { get; internal set; }

        public Exception Exception { get; internal set; }

        public Guid Id { get; private set; }

        public T Data { get; internal set; }
    }
}