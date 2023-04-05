using Game.Events;
using System.Collections;
using UnityEngine;

namespace Game.AssetsManagement
{
    /// <summary>
    /// Load files from bundles.
    /// </summary>
    public class BundleLoader : AssetSourceLoader
    {
        public override AssetSource source { get { return AssetSource.Bundle; } }

        public override void Load<T>(AssetContainer<T> container, bool async = false)
        {
            if (async)
            {
                EngineEvents.ExecuteCoroutine(AsyncLoadRoutine(container));
            }
            else
            {
                AssetBundle loadedBundle = AssetBundle.LoadFromFile(container.path);
                T asset = loadedBundle.LoadAsset<T>(container.fileName);

                container.SetAsset(asset);
                OnLoad(container);
            }
        }

        private IEnumerator AsyncLoadRoutine<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(container.path);
            yield return bundleRequest;

            AssetBundle loadedBundle = bundleRequest.assetBundle;
            container.assetBundle = loadedBundle;

            if (loadedBundle == null)
            {
                Debug.LogWarning($"Failed to load AssetBundle for file: [{container}]");
                yield break;
            }

            AssetBundleRequest assetRequest = loadedBundle.LoadAssetAsync<T>(container.fileName);
            yield return assetRequest;

            T file = assetRequest.asset as T;
            container.SetAsset(file);

            OnLoad(container);
        }
    }
}