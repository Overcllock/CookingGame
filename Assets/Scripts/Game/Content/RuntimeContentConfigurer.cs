using System;
using UnityEngine.AddressableAssets;

namespace Game.Content
{
    public class RuntimeContentConfigurer : ContentConfigurer
    {
        private const string DATABASE_TAG = "Database";

        public RuntimeContentConfigurer(IContentInstantiator instantiator) : base(instantiator)
        {
        }

        public override void ConfigureData(Action<ContentData> callback)
        {
            LoadRoutine(callback);
        }

        private async void LoadRoutine(Action<ContentData> callback)
        {
            var handle = Addressables.LoadAssetsAsync<ScrobjectDatabase>(DATABASE_TAG, null);
            await handle.Task;

            var data = PrepareContentData(handle.Result);
            callback.Invoke(data);

            Addressables.Release(handle);
        }
    }
}