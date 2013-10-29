using System.Collections.Generic;
using System.Linq;

namespace Greyhound.Filters
{
    public static class Filter
    {
        public static IEnumerable<IFilter<T>> NoFilter<T>()
        {
            return Enumerable.Empty<IFilter<T>>();
        }
    }
}