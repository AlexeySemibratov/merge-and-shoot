using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableUtils
{
    public static void DoForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
            action(item);
    }

    public static void DoForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
        for (int i = 0; i < enumerable.Count(); i++)
            action(enumerable.ElementAt(i), i);
    }
}
