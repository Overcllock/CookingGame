using System;
using System.Collections.Generic;

namespace Game.Content.Recipes
{
    public class RecipesContentMap : ContentMap
    {
        public EntryMap<RecipeEntry> recipes = new EntryMap<RecipeEntry>();

        protected override Dictionary<Type, EntryMap> GetMaps()
        {
            return new Dictionary<Type, EntryMap>
            {
                [typeof(RecipeEntry)] = recipes
            };
        }
    }
}