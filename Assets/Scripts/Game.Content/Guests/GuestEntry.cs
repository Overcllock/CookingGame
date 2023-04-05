using UnityEngine.AddressableAssets;

namespace Game.Content.Guests
{
    public class GuestEntry : ContentEntry
    {
        public AssetReferenceSprite icon;
        public int minRecipesCount;
        public int maxRecipesCount;
    }
}