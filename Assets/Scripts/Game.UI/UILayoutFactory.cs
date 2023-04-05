using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.UI
{
    public class UILayoutFactory
    {
        private Transform _mainCanvas;

        public UILayoutFactory(Transform mainCanvas)
        {
            _mainCanvas = mainCanvas;
        }
        
        public T Create<T>(string prefabName, Transform parent = null, string prefix = "[UI]") where T : UIBaseLayout
        {
            var prefab = Resources.Load<T>(prefabName);
            return Create(prefab, parent, prefix);
        }

        public T Create<T>(AssetReference reference, Transform parent = null, string prefix = "[UI]") where T : UIBaseLayout
        {
            var handle = reference.LoadAssetAsync<GameObject>();
            var go = handle.WaitForCompletion();

            return Create<T>(go, parent, prefix);
        }

        public T Create<T>(GameObject prefab, Transform parent = null, string prefix = "[UI]") where T : UIBaseLayout
        {
            T component = prefab.GetComponent<T>();
            if (component == null)
            {
                UnityEngine.Debug.LogError(
                    $"Could not create instance of type: [{typeof(T).Name}] - " +
                    $"prefab does not contain the required component");
                return null;
            }

            return Create(component, parent, prefix);
        }

        public T Create<T>(T prefab, Transform parent = null, string prefix = "[UI]", string name = null) where T : UIBaseLayout
        {
            T created = Instantiate(prefab, parent);

            string objectName = string.IsNullOrEmpty(name) ? prefab.name : name;
            created.rootObject.name = $"{prefix} {objectName}";

            return created;
        }

        private T Instantiate<T>(T prefab, Transform parent) where T : UIBaseLayout
        {
            return parent != null ?
                GameObject.Instantiate<T>(prefab, parent) :
                GameObject.Instantiate<T>(prefab, _mainCanvas);
        }
    }
}