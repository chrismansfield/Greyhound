using System.Collections.Generic;
using System.Linq;

namespace Greyhound
{
    internal static class FilterManager
    {
        public static IEnumerable<ISubscriber<T>> WhereFiltersMatch<T>(this IEnumerable<ISubscriber<T>> subscribers,
            IMessage<T> message)
        {
            return subscribers.Where(subscriber => EvaluateFilters(subscriber.GetFilters(), message));
        }

        private static bool EvaluateFilters<T>(IEnumerable<IFilter<T>> filters, IMessage<T> message)
        {
            return filters.All(f => f.Match(message));
        }
    }
}