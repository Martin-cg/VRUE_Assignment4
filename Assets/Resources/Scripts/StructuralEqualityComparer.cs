using System.Collections;
using System.Collections.Generic;

public class StructuralEqualityComparer<T> : IEqualityComparer<T> {
    public bool Equals(T x, T y) => StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
    public int GetHashCode(T obj) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);

    private static StructuralEqualityComparer<T> instance;
    public static StructuralEqualityComparer<T> Default => instance ??= new StructuralEqualityComparer<T>();
}
