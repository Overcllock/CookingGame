using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Content
{
    public abstract class ContentConfigurer : Predicated<Type>, IContentConfigurer
    {
        protected IContentInstantiator _instantiator;

        protected ContentConvertationAgent _converterAgent;
        protected BidirectionalMap<Type, Type> _associativeMap;

        public ContentConfigurer(IContentInstantiator instantiator)
        {
            _instantiator = instantiator;
            _converterAgent = new ContentConvertationAgent();

            _associativeMap = _converterAgent.GetAssociativeMap();
        }

        public abstract void ConfigureData(Action<ContentData> callback);

        protected ContentData PrepareContentData(IList<ScrobjectDatabase> databases)
        {
            var contentMaps = ConfigureMaps(databases);
            var settings = ConfigureModuleSettings(databases);

            return new ContentData
            {
                contentMaps = contentMaps,
                settings = settings
            };
        }

        private Dictionary<string, ContentMap> ConfigureMaps(IList<ScrobjectDatabase> databases)
        {
            var dbMap = GetScrobjectsDatabaseMap(databases);

            Dictionary<string, ContentMap> data = new Dictionary<string, ContentMap>();
            var mapTypes = ReflectionUtility.GetAllTypes<ContentMap>();

            for (int i = 0; i < mapTypes.Count; i++)
            {
                Type type = mapTypes[i];

                var instance = _instantiator.Instantiate(type);

                string moduleName = type.Namespace;
                if (dbMap.TryGetValue(moduleName, out var db))
                {
                    if (!CheckPredicates(db.GetType())) continue;

                    ConfigureMap(instance, db);
                    data[moduleName] = instance;
                }
                else
                {
                    Debug.LogError(
                        $"Detected inconsistent scrobject database amount. " +
                        $"Consider checking addressables.");

                    Debug.LogError(
                        $"Could not correctly configure content map: [ {type} ] ");
                }
            }

            return data;
        }

        private Dictionary<Type, ContentModuleSettings> ConfigureModuleSettings(IList<ScrobjectDatabase> databases)
        {
            Dictionary<Type, ContentModuleSettings> settings = new Dictionary<Type, ContentModuleSettings>();

            for (int i = 0; i < databases.Count; i++)
            {
                var db = databases[i];
                var dbType = db.GetType();

                if (!CheckPredicates(dbType)) continue;

                if (_associativeMap.TryGetBySecond(dbType, out var settingType) &&
                        _converterAgent.TryGetConverter(settingType, out var converter))
                {
                    var settingsEntry = converter.Convert<ContentModuleSettings>(db);
                    settings[settingType] = settingsEntry;
                }
            }

            return settings;
        }

        private void ConfigureMap(ContentMap contentMap, ScrobjectDatabase database)
        {
            contentMap.Initialize();

            var scrobjectsByType = GetScrobjectsByType(database);

            foreach (var kvp in scrobjectsByType)
            {
                var scrobjectType = kvp.Key;

                if (!_associativeMap.TryGetBySecond(scrobjectType, out var entryType))
                {
                    Debug.LogError($"No entry type was associated with scrobject type: {scrobjectType.Name}");
                    continue;
                }

                if (!CheckPredicates(scrobjectType)) continue;

                if (contentMap.allMaps.TryGetValue(entryType, out EntryMap entryMap))
                {
                    var scrobjects = kvp.Value;
                    PopulateEntryMap(entryType, entryMap, scrobjects);
                }
                else
                {
                    Debug.LogError($"Could not find valid entry map for type: {entryType.Name}");
                }
            }
        }

        private void PopulateEntryMap(Type entryType, EntryMap map, List<BaseSettingsScrobject> scrobjects)
        {
            map.Clear();

            if (!_converterAgent.TryGetConverter(entryType, out var converter))
            {
                Debug.LogWarning($"Missing converter for entries of type: {entryType.Name}");
                return;
            }

            for (int i = 0; i < scrobjects.Count; i++)
            {
                var scrobject = scrobjects[i];

                Precondition.MustNotBeNull(scrobject,
                    $"Detected null reference for scrobject " +
                    $"of type: {_associativeMap.GetByFirst(entryType).Name}");

                if (string.IsNullOrWhiteSpace(scrobject.id))
                {
                    Debug.LogError($"{scrobject.name} ({scrobject.GetType().Name}): 'id' not specified.", scrobject);
                    continue;
                }

                var converted = converter.Convert<ContentEntry>(scrobject);
                OnConverted(entryType, converted);
                map.Add(converted);
            }
        }

        private Dictionary<Type, List<BaseSettingsScrobject>> GetScrobjectsByType(ScrobjectDatabase database)
        {
            Dictionary<Type, List<BaseSettingsScrobject>> map = new Dictionary<Type, List<BaseSettingsScrobject>>();
            for (int i = 0; i < database.allScrobjects.Count; i++)
            {
                var scrobject = database.allScrobjects[i];

                if (scrobject == null)
                {
                    Debug.LogError($"Detected null scrobject in database: [ {database.GetType().Name} ] index: [ {i} ]");
                    continue;
                }

                var type = scrobject.GetType();

                if (!map.TryGetValue(type, out var list))
                {
                    list = new List<BaseSettingsScrobject>();
                    map[type] = list;
                }

                list.Add(scrobject);
            }

            return map;
        }

        private Dictionary<string, ScrobjectDatabase> GetScrobjectsDatabaseMap(IList<ScrobjectDatabase> databases)
        {
            var map = new Dictionary<string, ScrobjectDatabase>(databases.Count);
            for (int i = 0; i < databases.Count; i++)
            {
                var db = databases[i];
                var moduleName = db.GetType().Namespace;

                if (map.ContainsKey(moduleName))
                {
                    Debug.LogError($"Found scrobject database module collisions: [ {moduleName} ]", db);
                }
                else
                {
                    map[moduleName] = db;
                }
            }

            return map;
        }

        protected virtual void OnConverted(Type entryType, ContentEntry entry)
        {
        }
    }
}