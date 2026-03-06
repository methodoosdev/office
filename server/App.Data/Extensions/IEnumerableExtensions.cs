using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class IEnumerableExtensions
    {
        public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string param, string dir)
        {
            var propertyInfo = typeof(TSource).GetProperty(param);
            return (dir == "desc")
                ? source.OrderByDescending(x => propertyInfo.GetValue(x, null))
                : source.OrderBy(x => propertyInfo.GetValue(x, null));
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource>(this IOrderedEnumerable<TSource> source, string param, string dir)
        {
            var propertyInfo = typeof(TSource).GetProperty(param);
            return (dir == "desc")
                ? source.ThenByDescending(x => propertyInfo.GetValue(x, null))
                : source.ThenBy(x => propertyInfo.GetValue(x, null));
        }
    }
}