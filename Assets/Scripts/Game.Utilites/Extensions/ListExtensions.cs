using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    /// <summary>
    /// Always prefer Rand to System.Random
    /// when working with shared logic.
    /// </summary>
    public static T GetRandomValue<T>(this IList<T> list, Random random)
    {
        int index = random.Next(0, list.Count);
        return list[index];
    }

    public static bool IsNullOrEmpty(this ICollection collection)
    {
        return collection == null || collection.Count == 0;
    }

    public static List<T> AddRepeated<T>(this List<T> list, T item, int count)
    {
        list.AddRange(Enumerable.Repeat(item, count));
        return list;
    }

    public static List<T> AddRepeated<T>(this List<T> list, int index, int count)
    {
        if (index < 0 || index >= list.Count)
        {
            throw new System.InvalidOperationException($"Invalid operation index: [{index}] for list of count: [{list.Count}]");
        }

        list.AddRange(Enumerable.Repeat(list[index], count));
        return list;
    }

    public static List<T> FilterBy<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        if (predicate == null)
            throw new InvalidOperationException("Predicate cannot be null");

        var filtered = new List<T>();

        foreach (var nextElement in collection)
        {
            if (predicate.Invoke(nextElement))
            {
                filtered.Add(nextElement);
            }
        }

        return filtered;
    }

    public static List<T> FilterBy<T>(this List<T> list, Func<T, bool> predicate, out List<T> remainder)
    {
        if (predicate == null)
            throw new InvalidOperationException("Predicate cannot be null");

        var filtered = new List<T>();
        remainder = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            var nextElement = list[i];

            if (predicate.Invoke(nextElement))
            {
                filtered.Add(nextElement);
            }
            else
            {
                remainder.Add(nextElement);
            }
        }

        return filtered;
    }

    public static List<TConverted> Cast<TConverted, TOriginal>(this List<TOriginal> list) where TOriginal : TConverted
    {
        var castList = new List<TConverted>(list.Count);

        for (int i = 0; i < list.Count; i++)
        {
            castList.Add(list[i]);
        }

        return castList;
    }
}