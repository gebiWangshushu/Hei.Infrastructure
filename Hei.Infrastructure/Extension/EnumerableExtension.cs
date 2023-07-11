using System.Collections.Generic;
using System.Linq;

namespace Hei.Infrastructure
{
    public static class EnumerableExtension
    {
        public static bool IsNotNullOrEmpty<TSource>(this IEnumerable<TSource> list) => !IsNullOrEmpty(list);

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> list) => list == null || list.Any() == false;
    }
}