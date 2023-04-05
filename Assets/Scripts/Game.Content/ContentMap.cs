using System;
using System.Collections.Generic;

namespace Game.Content
{
    /// <summary>
    /// Map that contains module specific <see cref="EntryMap"/>s.
    /// <br/>Module is defined by shared namespace.
    /// </summary>
    public abstract class ContentMap
    {
        private Dictionary<Type, EntryMap> _allMaps;
        public Dictionary<Type, EntryMap> allMaps
        {
            get
            {
                if (_allMaps == null)
                {
                    _allMaps = GetMaps();
                }

                return _allMaps;
            }
        }

        public bool isConfigured { get { return _allMaps != null; } }

        public void Initialize()
        {
            if (_allMaps == null)
            {
                _allMaps = GetMaps();
            }
        }

        protected abstract Dictionary<Type, EntryMap> GetMaps();

        public T GetEntry<T>(string id) where T : ContentEntry
        {
            if (TryGetEntryMap<T>(out var entryMap))
            {
                return entryMap[id];
            }

            return null;
        }

        public EntryMap<T> GetEntryMap<T>() where T : ContentEntry
        {
            return (EntryMap<T>)allMaps[typeof(T)];
        }

        public bool TryGetEntryMap<T>(out EntryMap<T> map) where T : ContentEntry
        {
            if (allMaps.TryGetValue(typeof(T), out EntryMap retrievedMap))
            {
                map = (EntryMap<T>)retrievedMap;
                return true;
            }

            map = null;
            return false;
        }

        public bool TryGetEntry<T>(string id, out T entry) where T : ContentEntry
        {
            if (TryGetEntryMap(out EntryMap<T> entryMap))
            {
                return entryMap.TryGet(id, out entry);
            }

            entry = null;
            return false;
        }
    }
}