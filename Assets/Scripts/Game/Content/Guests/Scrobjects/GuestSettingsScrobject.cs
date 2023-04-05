using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Content.Guests
{
    [CreateAssetMenu(menuName = "Content/Guests/Recipe Settings", fileName = "Guest")]
    public class GuestSettingsScrobject : BaseSettingsScrobject
    {
        public AssetReferenceSprite icon;
        public int minRecipesCount;
        public int maxRecipesCount;
    }
}