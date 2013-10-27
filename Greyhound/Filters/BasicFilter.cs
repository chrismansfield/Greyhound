using System;

namespace Greyhound.Filters
{
    public class BasicFilter<T> : IFilter<T>
    {
        private readonly Func<IMessage<T>, bool> _match;

        public BasicFilter(Func<IMessage<T>, bool> match)
        {
            _match = match;
        }

        public bool Match(IMessage<T> message)
        {
            return _match(message);
        }
    }
}