using System;

namespace Game.AssetsManagement
{
    /// <summary>
    /// Loaders for different types of asset sources.
    /// </summary>
    public abstract class AssetSourceLoader
    {
        public abstract AssetSource source { get; }
        public event Action<AssetContainer> fileLoaded;

        public abstract void Load<T>(AssetContainer<T> container, bool async = false) where T : UnityEngine.Object;
        protected virtual void OnLoad(AssetContainer container)
        {
            fileLoaded?.Invoke(container);
        }

        public virtual void Clear()
        {
        }
    }
}