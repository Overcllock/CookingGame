using Game.Content.Recipes;
using UnityEngine;

namespace Game.Utilites
{
    public static class RecipesUtility
    {
        public static string[] GetUniqueRecipes(int count)
        {
            var map = ContentManager.GetMap<RecipesContentMap>();

            if (count > map.recipes.count)
            {
                Debug.LogError("Not enough available recipes");
                return null;
            }

            var recipes = map.recipes.GetEntries();
            var result = new string[count];

            for (int i = 0; i < count; i++)
            {
                var index = Random.Range(0, recipes.Count);
                var recipeEntry = recipes[index];

                result[i] = recipeEntry.id;
                
                recipes.RemoveAt(index);
            }

            return result;
        }
    }
}