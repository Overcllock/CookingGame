using System;
using System.Collections.Generic;

namespace Game.Content
{
    /// <summary>
    /// Map that holds all possible <see cref="ContentEntry"/>
    /// for a specific type of numerous content data.
    /// </summary>
    public abstract class EntryMap
    {
        public abstract void Clear();
        public abstract void Add(ContentEntry entry);
        public abstract List<ContentEntry> GetEntries();
        public abstract bool Contains(string id);
        public abstract bool TryGet(string id, out ContentEntry entry);
    }

    public class EntryMap<T> : EntryMap where T : ContentEntry
    {
        private Dictionary<string, T> _dict;

        public T this[string id]
        {
            get { return _dict[id]; }
            set { _dict[id] = value; }
        }

        public Dictionary<string, T>.ValueCollection values { get { return _dict.Values; } }
        public int count { get { return _dict.Count; } }

        public EntryMap(int capacity = 0)
        {
            _dict = new Dictionary<string, T>(capacity);
        }

        public bool TryGet(string id, out T value)
        {
            return _dict.TryGetValue(id, out value);
        }

        public override bool TryGet(string id, out ContentEntry entry)
        {
            if (_dict.TryGetValue(id, out var typedEntry))
            {
                entry = typedEntry;
                return true;
            }

            entry = null;
            return false;
        }
        
        public override void Add(ContentEntry entry)
        {
            _dict[entry.id] = (T)entry;
        }

        public void Add(T entry)
        {
            _dict[entry.id] = entry;
        }

        public override void Clear()
        {
            _dict.Clear();
        }

        public override bool Contains(string id)
        {
            return _dict.ContainsKey(id);
        }

        public bool Contains(T entry)
        {
            return Contains(entry.id);
        }

        public bool ContainsAny(Func<T, bool> predicate)
        {
            foreach (var entry in values)
            {
                if (predicate(entry)) return true;
            }

            return false;
        }

        public bool TryGetAll(Func<T, bool> predicate, out List<T> entries)
        {
            entries = new List<T>();
            foreach (var entry in values)
            {
                if (predicate(entry))
                {
                    entries.Add(entry);
                }
            }

            return entries.Count > 0;
        }

        public override List<ContentEntry> GetEntries()
        {
            var entries = new List<ContentEntry>(values.Count);
            foreach (var entry in values)
            {
                entries.Add(entry);
            }

            return entries;
        }
    }
}