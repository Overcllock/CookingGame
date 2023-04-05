using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.AssetsManagement
{
    public class AddressableUsageInfo
    {
        public int refCount { get; private set; }
        public string referenceId { get; private set; }
        public AsyncOperationHandle handle { get; private set; }

        public AddressableUsageInfo(string referenceId, AsyncOperationHandle handle)
        {
            this.referenceId = referenceId;
            this.handle = handle;
            refCount = 1;
        }

        public void AppendUsage()
        {
            if (CheckReleased()) return;
            refCount++;
        }

        public void RemoveUsage()
        {
            if (CheckReleased()) return;
            refCount--;
        }

        public void Release()
        {
            refCount = 0;
            if (referenceId != null)
            {
                Addressables.Release(handle);

                referenceId = null;
                handle = default;
            }
        }

        private bool CheckReleased()
        {
            bool released = refCount == 0;

            if (released)
            {
                Debug.LogWarning($"Trying to call emptied usage info.");
            }

            return released;
        }

        public override string ToString()
        {
            return
                $"<color=white>" +
                $"Reference id: [{referenceId}] " +
                $"Reference count: [{refCount}]" +
                $"</color>";
        }
    }
}