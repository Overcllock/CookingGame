using System;
using Game.Content.Guests;
using Game.Content.Recipes;

namespace Game.Content.Campaign
{
    public class LevelEntry : ContentEntry
    {
        public int time;
        public int ordersCount;
        public int ordersCapacity;
        public int minOrderTime;
        public int maxOrderTime;
        public bool randomizeGuests;
        public string[] availableGuests;
        public string[] recipesSources;
        public OrderSettings[] orders;
    }

    [Serializable]
    public class OrderSettings
    {
        [BoundId(typeof(GuestSettingsScrobject))]
        public string guestId;
        
        [BoundId(typeof(RecipeSettingsScrobject))]
        public string[] recipesIds;
        
        public int waitingTime;
    }
}