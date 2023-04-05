using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

using Object = UnityEngine.Object;

namespace Game.AssetsManagement
{
    public class AssetContainer : CustomYieldInstruction, IDisposable
    {
        private Action<AssetContainer> _callback;

        public byte[] rawBytes;
        public AssetBundle assetBundle;

        public int id { get; protected set; }
        public AssetSource source { get; protected set; }
        public string path { get; protected set; }
        public string fileName { get; protected set; }
        public AssetReference reference { get; protected set; }
        public string referenceId { get; protected set; }
        public bool isLoaded { get; protected set; }
        public bool isCancelled { get; protected set; }
        public bool isDisposed { get; protected set; }
        public override bool keepWaiting
        {
            get
            {
                if (isCancelled) return false;

                return !isLoaded;
            }
        }

        public event Action loaded;
        public event Action<AssetContainer> cancelled;
        public event Action<AssetContainer> disposed;

        public AssetContainer(int id, AssetArgs args, Action<AssetContainer> callback = null)
        {
            this.id = id;

            source = args.source;
            path = args.path;
            fileName = args.fileName;
            reference = args.reference;

            referenceId = reference != null ?
               reference.AssetGUID :
               path;

            _callback = callback;
        }

        internal void MarkLoaded()
        {
            isLoaded = true;

            if (isCancelled)
            {
                Dispose();
            }
            else
            {
                InvokeCallback();
                loaded?.Invoke();
            }
        }

        protected virtual void InvokeCallback()
        {
            _callback?.Invoke(this);
            _callback = null;
        }

        public AssetContainer<T> Convert<T>() where T : UnityEngine.Object
        {
            return (AssetContainer<T>)this;
        }

        public virtual void Dispose()
        {
            if (isDisposed) return;

            bool isToCancel =
                !isLoaded &&
                !isCancelled;

            if (isToCancel)
            {
                isCancelled = true;
                cancelled?.Invoke(this);
            }
            else
            {
                isDisposed = true;
                disposed?.Invoke(this);

                _callback = null;
                disposed = null;
                loaded = null;
                reference = null;
                assetBundle = null;
                rawBytes = null;
            }
        }

        public override string ToString()
        {
            return
                $"<color=white>" +
                $"Id: [{id}] " +
                $"Path: [{path}] " +
                $"Reference: [{reference?.AssetGUID}] " +
                $"Source: [{source}]" +
                $"</color>";
        }
    }

    public class AssetContainer<T> : AssetContainer where T : Object
    {
        private Action<AssetContainer<T>> _callback;

        public T asset { get; private set; }
        public bool isComponent { get { return typeof(T).IsSubclassOf(typeof(Component)); } }

        public AssetContainer(int id, AssetArgs<T> args, Action<AssetContainer<T>> callback = null) : base(id, args)
        {
            _callback = callback;
        }

        internal void SetAsset(T asset)
        {
            this.asset = asset;
        }

        protected override void InvokeCallback()
        {
            _callback?.Invoke(this);
            _callback = null;
        }

        public T Instantiate(Transform parent = null)
        {
            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Trying to instantiate from empty asset container: " +
                    $"[ {GetType().Name} / {referenceId} ]");
            }

            if (parent != null)
            {
                return Object.Instantiate(asset, parent);
            }

            return Object.Instantiate(asset);
        }

        public T Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (asset == null)
            {
                throw new InvalidOperationException(
                    $"Trying to instantiate from empty asset container: " +
                    $"[ {GetType().Name} / {referenceId} ]");
            }

            if (parent != null)
            {
                return Object.Instantiate(asset, position, rotation, parent);
            }

            return Object.Instantiate(asset, position, rotation);
        }

        public T Instantiate(PositionAndRotation pos, Transform parent = null)
        {
            return Instantiate(pos.position, pos.rotation, parent);
        }

        public override string ToString()
        {
            return base.ToString() +
                $"<color=white>" +
                $" Type: [{typeof(T)}]" +
                $" Is Component: [{isComponent}]" +
#if UNITY_EDITOR
                $" Asset: [{reference?.editorAsset?.name}]" +
#endif
                $"</color>";
        }

        public override void Dispose()
        {
            base.Dispose();
            asset = null;
            _callback = null;
        }
    }
}