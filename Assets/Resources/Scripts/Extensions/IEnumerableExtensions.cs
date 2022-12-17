using System.Linq;
using System.Collections.Generic;

public static class IEnumerableExtensions {
    public static IEnumerable<(TFirst, TSecond)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> source, IEnumerable<TSecond> other) =>
        source.Zip(other, (a, b) => (a, b));
}
