#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseUtility
{
    public static T GetAsset<T>(string path = null) where T : Object
    {
        var guids = GetAssetsGuids<T>(path);

        if (guids.IsNullOrEmpty())
        {
            Debug.LogError($"Could not find valid guid for asset of type: [ {typeof(T)} ] at path: [ {path} ]");
            return null;
        }

        if (guids.Length > 1)
        {
            Debug.LogWarning($"Found more than one asset of type: [ {typeof(T)} ]");
        }

        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids.First()));
    }

    public static Object[] GetAssets(System.Type type, string path = null)
    {
        var guids = GetAssetsGuids(type, path);

        var assets = new Object[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            Object asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[i]), type);
            assets[i] = asset;
        }

        return assets;
    }

    public static T[] GetAssets<T>(string path = null) where T : Object
    {
        var guids = GetAssetsGuids<T>(path);

        var assets = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            assets[i] = asset;
        }

        return assets;
    }

    public static T GetPrefab<T>(string path) where T : MonoBehaviour
    {
        if (path.IsNullOrEmpty())
        {
            Debug.LogWarning($"Empty path for prefab of type: [ {typeof(T)} ]");
            return null;
        }

        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (PrefabUtility.IsPartOfAnyPrefab(asset))
        {
            var component = asset.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning(
                    $"Could not find valid component of type [ {typeof(T)} ] " +
                    $"on prefab at path: [ {path} ]");
            }

            return component;
        }

        Debug.LogWarning($"Could not find prefab at path: [ {path} ]");
        return null;
    }

    public static ScriptableObject CreateScrobject(System.Type type, string path, string assetName, bool saveAssets = true)
    {
        string absolutePath = IOUtility.GetAbsolutePath(path);
        if (!System.IO.Directory.Exists(absolutePath))
        {
            System.IO.Directory.CreateDirectory(absolutePath);
        }

        ScriptableObject asset = ScriptableObject.CreateInstance(type);
        assetName = assetName.Contains(".asset") ?
            assetName :
            assetName + ".asset";

        string finalPath = $"{path}/{assetName}";
        AssetDatabase.CreateAsset(asset, finalPath);

        if (saveAssets)
        {
            AssetDatabase.SaveAssets();
        }

        return asset;
    }

    public static string[] GetAssetsGuids<T>(string path = null)
    {
        return GetAssetsGuids(typeof(T), path);
    }

    public static string[] GetAssetsGuids(System.Type type, string path = null)
    {
        return string.IsNullOrEmpty(path) ?
            AssetDatabase.FindAssets($"t:{type.Name}") :
            AssetDatabase.FindAssets($"t:{type.Name}", new string[] { path });
    }
}

#endif