using Game.AssetsManagement;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

/// <summary>
/// Facade for game files loading management.
/// Provides API for loading files of different types and storage formats.
/// </summary>
public class AssetsLoader : StaticWrapper<IAssetsManager>
{
    public static T LoadSync<T>(AssetReference reference) where T : UnityEngine.Object
    {
        return Load<T>(reference, async: false).asset;
    }
    
    public static T LoadSync<T>(string path) where T : UnityEngine.Object
    {
        return Load<T>(path, async: false).asset;
    }    

    public static AssetContainer<T> Load<T>(AssetArgs<T> args) where T : UnityEngine.Object
    {
        if (InitializationCheck())
        {
            return _instance.LoadAsset(args);
        }

        return null;
    }

    public static AssetContainer<T> Load<T>(AssetReference reference,
        Action<AssetContainer<T>> callback = null, bool async = true) where T : UnityEngine.Object
    {
        if (InitializationCheck())
        {
            return _instance.LoadAsset(reference, callback, async);
        }

        return null;
    }

    public static AssetContainer<T> Load<T>(string path,
        AssetSource source = AssetSource.Addressable,
        Action<AssetContainer<T>> callback = null,
        string fileName = null,
        bool async = true) where T : UnityEngine.Object
    {
        if (InitializationCheck())
        {
            return _instance.LoadAsset(path, fileName, source, callback, async);
        }

        return null;
    }

    public static UniTask<T> LoadAsync<T>(AssetReference reference) where T : UnityEngine.Object
    {
        var tcs = new UniTaskCompletionSource<T>();
        
        Load<T>(reference, container =>
        {
            tcs.TrySetResult(container.asset);
        });

        return tcs.Task;
    }

    public static void ClearSource(AssetSource source)
    {
        if (InitializationCheck())
        {
            _instance.ClearSource(source);
        }
    }
    public static void Clear()
    {
        if (InitializationCheck())
        {
            _instance.Clear();
        }
    }

    public static void Terminate()
    {
        if (_instance != null)
        {
            _instance.Dispose();
            _instance = null;
        }
    }
}