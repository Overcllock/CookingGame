//using Game.Events;
using Game.Events;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Game.AssetsManagement
{
    public class ResourcesLoader : AssetSourceLoader
    {
        private RoutineQueue _asyncRoutines;

        public override AssetSource source { get { return AssetSource.Resources; } }

        public override void Load<T>(AssetContainer<T> container, bool async = false)
        {
            if (async)
            {
                if (_asyncRoutines == null)
                {
                    _asyncRoutines = new RoutineQueue(true);
                }

                _asyncRoutines.Enqueue(AsyncLoadRoutine(container));
            }
            else
            {
                string assetLocation = GetAssetLocation(container);
                T file = Resources.Load<T>(assetLocation);
                container.SetAsset(file);

                OnLoad(container);
            }
        }

        private IEnumerator AsyncLoadRoutine<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            string assetLocation = GetAssetLocation(container);
            ResourceRequest asyncLoad = Resources.LoadAsync<T>(assetLocation);
            yield return asyncLoad;

            T file = asyncLoad.asset as T;
            container.SetAsset(file);

            OnLoad(container);
        }

        private string GetAssetLocation(AssetContainer container)
        {
            return string.IsNullOrEmpty(container.fileName) ? container.path :
                   string.IsNullOrEmpty(container.path) ? container.fileName :
                   Path.Combine(container.path, container.fileName);
        }
    }
}