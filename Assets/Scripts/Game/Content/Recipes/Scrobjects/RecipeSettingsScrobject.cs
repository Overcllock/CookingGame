using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Content.Recipes
{
    [CreateAssetMenu(menuName = "Content/Recipes/Recipe Settings", fileName = "Recipe")]
    public class RecipeSettingsScrobject : BaseSettingsScrobject
    {
        public AssetReferenceSprite icon;
    }
}