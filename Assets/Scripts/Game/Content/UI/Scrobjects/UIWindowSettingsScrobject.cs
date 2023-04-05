using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Content.UI
{
    [CreateAssetMenu(menuName = "Content/GUI/Window Settings", fileName = "Window")]
    [DefinedConstants]
    public class UIWindowSettingsScrobject : BaseSettingsScrobject
    {
        public int priority;

        public bool showOnOverlay;

        [Space]
        public AssetReference prefabReference;

        [Space]
        public bool useFader;

        [ShowIf("useFader")]
        public float faderDuration = 0.5f;
        
        [ShowIf("useFader")]
        public float faderDelay;
    }
}