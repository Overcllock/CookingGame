using Game.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.AssetsManagement
{
    public class AddressableLoader : AssetSourceLoader
    {
        private Dictionary<string, AddressableUsageInfo> _usages = new Dictionary<string, AddressableUsageInfo>();
        private Dictionary<string, Queue<Action>> _queuedRequests = new Dictionary<string, Queue<Action>>();
        private Dictionary<string, IEnumerator> _ongoingRoutines = new Dictionary<string, IEnumerator>();

        private Dictionary<string, AssetContainer> _processingContainers = new Dictionary<string, AssetContainer>();

        public override AssetSource source { get { return AssetSource.Addressable; } }

        public override void Load<T>(AssetContainer<T> container, bool async = true)
        {
            if (string.IsNullOrEmpty(container.referenceId))
            {
                Debug.LogError($"Trying to load asset with empty reference: {container}");
                return;
            }

            if (!TryRetrieveFromStash(container, async))
            {
                if (async)
                {
                    if (_ongoingRoutines.ContainsKey(container.referenceId))
                    {
                        Debug.LogError(
                            $"Trying to perform multiple load operations " +
                            $"for the same asset: [ {typeof(T)} ] {container}");
                        return;
                    }

                    var routine = AsyncLoadRoutine(container);
                    _ongoingRoutines.Add(container.referenceId, routine);

                    EngineEvents.ExecuteCoroutine(routine);
                }
                else
                {
                    SyncLoad(container);
                }
            }
        }

        private void SyncLoad<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            if (!ValidateContainer(container)) return;

            T asset = GetAssetSync(container);
            ManageLoadQueue(asset, container);
        }

        private T GetAssetSync<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            if (container.isComponent)
            {
                AsyncOperationHandle<GameObject> handle = GetHandle<GameObject>(container);
                AddUsage(container.referenceId, handle);

                GameObject go = handle.WaitForCompletion();
                return go.GetComponent<T>();
            }
            else
            {
                AsyncOperationHandle<T> handle = GetHandle<T>(container);
                AddUsage(container.referenceId, handle);

                return handle.WaitForCompletion();
            }
        }

        private IEnumerator AsyncLoadRoutine<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            if (!ValidateContainer(container)) yield break;

            _processingContainers[container.referenceId] = container;

            if (container.isComponent)
            {
                yield return ComponentLoadRoutine(container);
            }
            else
            {
                yield return AssetLoadRoutine(container);
            }

            _processingContainers.Remove(container.referenceId);
        }

        private IEnumerator ComponentLoadRoutine<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            AsyncOperationHandle<GameObject> handle = GetHandle<GameObject>(container);
            AddUsage(container.referenceId, handle);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ManageLoadQueue(handle.Result.GetComponent<T>(), container);
            }
            else
            {
                Debug.LogError($"Failed to load component of type [ {typeof(T)} ] container info: {container}");
            }
        }

        private IEnumerator AssetLoadRoutine<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = GetHandle<T>(container);
            AddUsage(container.referenceId, handle);

            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                ManageLoadQueue(handle.Result, container);
            }
            else
            {
                Debug.LogError($"Failed to load asset of type [ {typeof(T)} ] container info: {container}");
            }
        }

        private void ManageLoadQueue<T>(T asset, AssetContainer<T> container) where T : UnityEngine.Object
        {
            if (asset == null)
            {
                Debug.LogError($"Loaded null asset: {container}");
            }

            LoadComplete(asset, container);

            if (_queuedRequests.TryGetValue(container.referenceId, out var queue))
            {
                while (queue.Count > 0)
                {
                    var queuedLoad = queue.Dequeue();

                    queuedLoad.Invoke();
                }
            }

            _ongoingRoutines.Remove(container.referenceId);
        }

        private void LoadComplete<T>(T asset, AssetContainer<T> container) where T : UnityEngine.Object
        {
            container.disposed += HandleContainerDisposed;

            container.SetAsset(asset);
            OnLoad(container);
        }

        private bool TryRetrieveFromStash<T>(AssetContainer<T> container, bool async) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(container.referenceId)) return false;

            if (_usages.TryGetValue(container.referenceId, out AddressableUsageInfo usage))
            {
                usage.AppendUsage();

                Action queuedLoad = () =>
                {
                    var cachedUsage = _usages[container.referenceId];

                    if (container.isComponent)
                    {
                        var typedHandle = cachedUsage.handle.Convert<GameObject>();
                        LoadComplete(typedHandle.Result.GetComponent<T>(), container);
                    }
                    else
                    {
                        var typedHandle = cachedUsage.handle.Convert<T>();
                        LoadComplete(typedHandle.Result, container);
                    }
                };

                if (usage.handle.IsDone)
                {
                    queuedLoad.Invoke();
                }
                else
                {
                    if (!_queuedRequests.TryGetValue(container.referenceId, out var queue))
                    {
                        queue = new Queue<Action>();
                        _queuedRequests[container.referenceId] = queue;
                    }

                    queue.Enqueue(queuedLoad);

                    if (!async)
                    {
                        ReconfigureAsyncRequestToSync<T>(container.referenceId, usage);
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// If we try to load asset synchronously while its still loading asynchronously
        /// it will create request collision.
        /// We also cannot queue it, because synchronous load requires result right away.
        /// Therefore, we need to reconfigure previous async load into sync, and
        /// continue execution with the same request order.
        /// </summary>
        private void ReconfigureAsyncRequestToSync<T>(string referenceId, AddressableUsageInfo usage) where T : UnityEngine.Object
        {
            Precondition.MustBeTrue(
                _processingContainers.TryGetValue(referenceId, out var container),
                $"Could not find current request for asset load: [{referenceId}]");

            Precondition.MustBeTrue(
                _ongoingRoutines.TryGetValue(referenceId, out var currentRoutine),
                $"Could not find current routine for asset load: [{referenceId}]");

            EngineEvents.CancelCoroutine(currentRoutine);       //1. cancel ongoing routine
            _ongoingRoutines.Remove(container.referenceId);

            usage.Release();                                    //2. release current occupied usage
            _usages.Remove(container.referenceId);

            SyncLoad(container.Convert<T>());                   //3. start sync load
        }

        private AddressableUsageInfo AddUsage(string referenceId, AsyncOperationHandle handle)
        {
            AddressableUsageInfo usage = new AddressableUsageInfo(referenceId, handle);

            _usages[referenceId] = usage;

            return usage;
        }

        private void HandleContainerDisposed(AssetContainer container)
        {
            container.disposed -= HandleContainerDisposed;

            if (_usages.Count == 0) return;

            if (!_usages.TryGetValue(container.referenceId, out var usage))
            {
                Debug.LogError($"Trying to remove non existent usage info: {usage}");
                return;
            }

            usage.RemoveUsage();
            if (usage.refCount == 0)
            {
                usage.Release();
                _usages.Remove(container.referenceId);
            }
        }

        private AsyncOperationHandle<T> GetHandle<T>(AssetContainer container) where T : UnityEngine.Object
        {
            try
            {
                return container.reference != null ?
                Addressables.LoadAssetAsync<T>(container.reference) :
                Addressables.LoadAssetAsync<T>(container.path);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not get operation handle for asset - " +
                    $"reference: [ {container.reference} ] " +
                    $"path: [ {container.path} ] " +
                    $"message: [ {e.Message} ]");
            }
        }

        private bool ValidateContainer<T>(AssetContainer<T> container) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(container.referenceId))
            {
                Debug.LogError($"Could not find valid reference id for requested asset: {container}");
                return false;
            }

            if (_usages.ContainsKey(container.referenceId))
            {
                Debug.LogError($"Trying to load same asset package multiple times: {container}");
                return false;
            }

            return true;
        }

        public override void Clear()
        {
            foreach (var routine in _ongoingRoutines.Values)
            {
                EngineEvents.CancelCoroutine(routine);
            }

            foreach (var processingContainer in _processingContainers.Values)
            {
                processingContainer.Dispose();
            }

            foreach (var info in _usages.Values)
            {
                info.Release();
            }

            _processingContainers.Clear();
            _ongoingRoutines.Clear();
            _queuedRequests.Clear();
            _usages.Clear();
        }
    }
}