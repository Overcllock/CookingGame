using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Allocates more memory, but makes lookup operations faster.
/// Also checks for duplicates when adding new values,
/// and for occurence when removing.
/// </summary>
public class HashList<T> : IEnumerable<T>
{
    private List<T> _list;
    private HashSet<T> _hashSet;

    public T this[int index]
    {
        get { return _list[index]; }
        set { Add(value); }
    }

    public int Count { get { return _list.Count; } }

    public event Action<T> added;
    public event Action<T> removed;

    public HashList()
    {
        _list = new List<T>();
        _hashSet = new HashSet<T>();
    }

    public HashList(int capacity)
    {
        _list = new List<T>(capacity);
        _hashSet = new HashSet<T>();
    }

    public bool Contains(T value)
    {
        return _hashSet.Contains(value);
    }

    public bool Add(T value)
    {
        if (!Contains(value))
        {
            _list.Add(value);
            _hashSet.Add(value);

            added?.Invoke(value);
            return true;
        }

        return false;
    }

    public bool Remove(T value)
    {
        if (Contains(value))
        {
            _list.Remove(value);
            _hashSet.Remove(value);

            removed?.Invoke(value);
            return true;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}