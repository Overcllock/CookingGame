using System;
using UnityEngine.AddressableAssets;

namespace Game.AssetsManagement
{
    public enum AssetSource
    {
        None,

        Resources,
        Bundle,
        Addressable,
        StreamingAssets
    }

    public class AssetArgs
    {
        public bool async = true;

        public string path;
        public string fileName;
        public AssetReference reference;
        public AssetSource source;
    }

    public class AssetArgs<T> : AssetArgs where T : UnityEngine.Object
    {
        public Action<AssetContainer<T>> callback;
    }

    public interface IAssetsManager : IDisposable, IClearable
    {
        AssetContainer<T> LoadAsset<T>(AssetArgs<T> args) where T : UnityEngine.Object;

        void ClearSource(AssetSource source);
    }

    public static class AssetsManagerExtensions
    {
        public static AssetContainer<T> LoadAsset<T>(this IAssetsManager assetsManager,
            AssetReference reference,
            Action<AssetContainer<T>> callback = null,
            bool async = true) where T : UnityEngine.Object
        {
            var args = new AssetArgs<T>
            {
                async = async,
                source = AssetSource.Addressable,
                reference = reference,
                callback = callback
            };

            return assetsManager.LoadAsset(args);
        }

        public static AssetContainer<T> LoadAsset<T>(this IAssetsManager assetsManager,
            string path,
            string fileName = null,
            AssetSource source = AssetSource.Addressable,
            Action<AssetContainer<T>> callback = null,
            bool async = false) where T : UnityEngine.Object
        {
            var args = new AssetArgs<T>
            {
                async = async,
                path = path,
                fileName = fileName,
                source = source,
                callback = callback
            };

            return assetsManager.LoadAsset(args);
        }
    }
}