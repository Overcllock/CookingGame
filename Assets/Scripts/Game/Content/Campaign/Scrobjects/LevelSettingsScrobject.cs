using Game.Content.Guests;
using Game.Content.Recipes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Content.Campaign
{
    [CreateAssetMenu(menuName = "Content/Campaign/Level Settings", fileName = "Level")]
    public class LevelSettingsScrobject : BaseSettingsScrobject
    {
        public int time;
        
        [Space]
        [ShowIf(nameof(ordersEmpty))]
        public int ordersCount;
        
        public int ordersCapacity;

        [Space]
        [ShowIf(nameof(ordersEmpty))]
        public int minOrderTime;
        
        [ShowIf(nameof(ordersEmpty))]
        public int maxOrderTime;
        
        [Space]
        public bool randomizeGuests;
        
        [Space]
        [BoundId(typeof(GuestSettingsScrobject))]
        [ShowIf(nameof(hasCustomGuests))]
        public string[] availableGuests;
        
        [Space]
        [BoundId(typeof(RecipeSettingsScrobject))]
        public string[] recipesSources;

        [Space]
        public OrderSettings[] orders;

        public bool ordersEmpty => orders.IsNullOrEmpty();
        public bool hasCustomGuests => !randomizeGuests;
    }
}