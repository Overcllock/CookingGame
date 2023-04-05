using UnityEngine.AddressableAssets;

namespace Game.Content.UI
{
    public class UIWindowEntry : ContentEntry
    {
        public int priority;
        public bool showOnOverlay;
        public AssetReference prefabReference;

        public bool useFader;
        public float faderDuration;
        public float faderDelay;
    }
}