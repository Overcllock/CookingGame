using Game.Content;
using System;

public class ContentManager : StaticWrapper<IContentManagementSystem>
{
    public static bool isLoaded { get { return _instance != null && _instance.isLoaded; } }

    public static void OnContentLoaded(Action action)
    {
        if (InitializationCheck())
        {
            _instance.OnContentLoaded(action);
        }
    }

    public static void GetEntry<T>(out T entry, string id) where T : ContentEntry
    {
        entry = null;
        
        if (InitializationCheck())
        {
            entry = _instance.GetEntry<T>(id);
        }
    }

    
    public static T GetEntry<T>(string id) where T : ContentEntry
    {
        if (InitializationCheck())
        {
            return _instance.GetEntry<T>(id);
        }

        return null;
    }

    public static bool TryGetEntry<T>(string id, out T entry) where T : ContentEntry
    {
        if (InitializationCheck())
        {
            return _instance.TryGetEntry(id, out entry);
        }

        entry = null;
        return false;
    }

    public static T GetMap<T>() where T : ContentMap
    {
        if (InitializationCheck())
        {
            return _instance.GetMap<T>();
        }

        return null;
    }

    public static bool TryGetMap<T>(out T map) where T : ContentMap
    {
        if (InitializationCheck())
        {
            return _instance.TryGetMap(out map);
        }

        map = null;
        return false;
    }

    public static EntryMap<T> GetEntryMap<T>() where T : ContentEntry
    {
        if (InitializationCheck())
        {
            return _instance.GetEntryMap<T>();
        }

        return null;
    }

    public static bool TryGetEntryMap<T>(out EntryMap<T> map) where T : ContentEntry
    {
        if (InitializationCheck())
        {
            return _instance.TryGetEntryMap<T>(out map);
        }

        map = null;
        return false;
    }

    public static T GetSettings<T>() where T : ContentModuleSettings
    {
        if (InitializationCheck())
        {
            return _instance.GetSettings<T>();
        }

        return null;
    }

    public static bool TryGetSettings<T>(out T settings) where T : ContentModuleSettings
    {
        if (InitializationCheck())
        {
            return _instance.TryGetSettings(out settings);
        }

        settings = null;
        return false;
    }
}