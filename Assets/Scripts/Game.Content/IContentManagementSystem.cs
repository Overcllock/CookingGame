using System;

namespace Game.Content
{
    public interface IContentManagementSystem
    {
        T GetMap<T>() where T : ContentMap;
        T GetEntry<T>(string id) where T : ContentEntry;
        T GetSettings<T>() where T : ContentModuleSettings;
        EntryMap<T> GetEntryMap<T>() where T : ContentEntry;

        bool isLoaded { get; }
        void OnContentLoaded(Action action);
    }

    public static class ContentManagerExtensions
    {
        public static bool TryGetMap<T>(this IContentManagementSystem system, out T map) where T : ContentMap
        {
            try
            {
                map = system.GetMap<T>();
                return map != null;
            }
            catch
            {
                map = null;
                return false;
            }
        }

        public static bool TryGetEntry<T>(this IContentManagementSystem system, string id, out T entry) where T : ContentEntry
        {
            try
            {
                entry = system.GetEntry<T>(id);
                return entry != null;
            }
            catch
            {
                entry = null;
                return false;
            }
        }

        public static bool TryGetEntryMap<T>(this IContentManagementSystem system, out EntryMap<T> entryMap) where T : ContentEntry
        {
            try
            {
                entryMap = system.GetEntryMap<T>();
                return entryMap != null;
            }
            catch
            {
                entryMap = null;
                return false;
            }
        }

        public static bool TryGetSettings<T>(this IContentManagementSystem system, out T settings) where T : ContentModuleSettings
        {
            try
            {
                settings = system.GetSettings<T>();
                return settings != null;
            }
            catch
            {
                settings = null;
                return false;
            }
        }
    }
}