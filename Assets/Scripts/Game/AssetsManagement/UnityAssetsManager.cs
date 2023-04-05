using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.AssetsManagement
{
    public class UnityAssetsManager : IAssetsManager
    {
        private Dictionary<AssetSource, AssetSourceLoader> _loaders;
        private Dictionary<int, AssetContainer> _queued;

        private Incrementer _incrementer;

        public event Action<AssetContainer> fileLoaded;

        public UnityAssetsManager()
        {
            _queued = new Dictionary<int, AssetContainer>();
            _incrementer = new Incrementer(1);

            var loadersList = ReflectionUtility.InstantiateAllTypes<AssetSourceLoader>();
            _loaders = new Dictionary<AssetSource, AssetSourceLoader>(loadersList.Count);

            for (int i = 0; i < loadersList.Count; i++)
            {
                AssetSourceLoader nextLoader = loadersList[i];
                _loaders[nextLoader.source] = nextLoader;

                nextLoader.fileLoaded += FileLoadedHandler;
            }
        }

        public AssetContainer<T> LoadAsset<T>(AssetArgs<T> args) where T : UnityEngine.Object
        {
            AssetContainer<T> container = new AssetContainer<T>(_incrementer.Get(), args, args.callback);
            _queued[container.id] = container;

            if (_loaders.TryGetValue(container.source, out AssetSourceLoader loader))
            {
                loader.Load(container, args.async);
            }
            else
            {
                Debug.LogError($"Found inaccessable file loader type: [{container.source}]");
            }

            return container;
        }

        private void FileLoadedHandler(AssetContainer container)
        {
            if (_queued.Remove(container.id))
            {
                container.MarkLoaded();
                fileLoaded?.Invoke(container);
            }
        }

        public void ClearSource(AssetSource source)
        {
            if (_loaders.TryGetValue(source, out var loader))
            {
                loader.Clear();
            }
        }

        public void Clear()
        {
            foreach (AssetSourceLoader loader in _loaders.Values)
            {
                loader.Clear();
            }
        }

        public void Dispose()
        {
            foreach (AssetSourceLoader loader in _loaders.Values)
            {
                loader.fileLoaded -= FileLoadedHandler;
                //loader.Clear(); //Temporary solution for GLAD-1913. Assets dispose before destroy unity objects
            }
        }
    }
}