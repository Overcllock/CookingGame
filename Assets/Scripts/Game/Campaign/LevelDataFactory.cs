using Game.Content.Campaign;
using Game.Content.Guests;
using Game.Utilites;
using UnityEngine;

namespace Game.Campaign
{
    public static class LevelDataFactory
    {
        public static LevelClientModel CreateLevel(LevelEntry entry)
        {
            return new LevelClientModel(entry);
        }

        public static OrderSettings[] GenerateOrders(LevelEntry entry)
        {
            var orders = new OrderSettings[entry.ordersCount];

            for (int i = 0; i < orders.Length; i++)
            {
                if (entry.availableGuests.IsNullOrEmpty())
                {
                    Debug.LogError("Can't generate orders: availableGuests is empty");
                    return null;
                }

                var guestId = entry.availableGuests[Random.Range(0, entry.availableGuests.Length)];

                if (!ContentManager.TryGetEntry(guestId, out GuestEntry guestEntry))
                {
                    Debug.LogError($"Can't generate orders: guest entry with id {guestId} is not exists");
                    return null;
                }

                var recipesCount = Random.Range(guestEntry.minRecipesCount - 1, guestEntry.maxRecipesCount) + 1;
                var recipes = RecipesUtility.GetUniqueRecipes(recipesCount);
                
                var settings = new OrderSettings
                {
                    waitingTime = Random.Range(entry.minOrderTime, entry.maxOrderTime),
                    guestId = guestId,
                    recipesIds = recipes
                };

                orders[i] = settings;
            }

            return orders;
        }

        public static OrderClientModel CreateOrder(OrderSettings settings)
        {
            return new OrderClientModel(settings);
        }
    }
}