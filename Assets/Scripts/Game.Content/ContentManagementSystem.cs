using System;
using System.Collections.Generic;

namespace Game.Content
{
    public class ContentManagementSystem : IContentManagementSystem
    {
        private IContentConfigurer _configurer;
        private Action _contentLoadedCallback;

        public Dictionary<Type, ContentMap> mapsByType { get; private set; }
        public Dictionary<string, ContentMap> mapsByModule { get; private set; }
        public Dictionary<Type, ContentModuleSettings> settingsByType { get; private set; }

        public bool isLoaded { get; private set; }

        public ContentManagementSystem(IContentConfigurer configurer)
        {
            _configurer = configurer;
            _configurer.ConfigureData(DataLoadedCallback);
        }

        public void OnContentLoaded(Action action)
        {
            if (isLoaded)
            {
                action.Invoke();
            }
            else
            {
                _contentLoadedCallback += action;
            }
        }

        public T GetEntry<T>(string id) where T : ContentEntry
        {
            Validate("GetEntry");

            string moduleName = typeof(T).Namespace;

            if (mapsByModule.TryGetValue(moduleName, out ContentMap map) &&
                map.TryGetEntry(id, out T entry))
            {
                return entry;
            }

            throw new Exception($"Could not find entry of type: [ {typeof(T).Name} ] with id: [ {id} ]");
        }

        public T GetMap<T>() where T : ContentMap
        {
            Validate("GetMap");

            if (mapsByType.TryGetValue(typeof(T), out var map))
            {
                return (T)map;
            }

            return null;
        }

        public T GetSettings<T>() where T : ContentModuleSettings
        {
            Validate("GetSettings");

            if (settingsByType.TryGetValue(typeof(T), out var settings))
            {
                return (T)settings;
            }

            throw new Exception($"Could not find ModuleSettings of type: [ {typeof(T).Name} ]");
        }

        public EntryMap<T> GetEntryMap<T>() where T : ContentEntry
        {
            Validate("GetEntryMap");

            string moduleName = typeof(T).Namespace;

            if (mapsByModule.TryGetValue(moduleName, out ContentMap map) &&
                map.TryGetEntryMap<T>(out var entryMap))
            {
                return entryMap;
            }

            throw new Exception($"Could not find EntryMap of type: [ {typeof(T)} ]");
        }

        private void DataLoadedCallback(ContentData data)
        {
            mapsByModule = data.contentMaps;
            settingsByType = data.settings;

            mapsByType = new Dictionary<Type, ContentMap>(mapsByModule.Count);
            foreach (var contentMap in mapsByModule.Values)
            {
                mapsByType[contentMap.GetType()] = contentMap;
            }

            isLoaded = true;
            _contentLoadedCallback?.Invoke();
            _contentLoadedCallback = null;
        }

        private void Validate(string message)
        {
            if (!isLoaded)
            {
                throw new InvalidOperationException($"Failed to perform content operation [ {message} ] - content is not loaded!");
            }
        }
    }
}