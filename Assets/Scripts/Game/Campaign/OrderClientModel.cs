using System;
using System.Collections.Generic;
using Game.Content.Campaign;

namespace Game.Campaign
{
    public class OrderClientModel
    {
        private readonly List<string> _recipes;
        
        public List<string> recipes { get { return _recipes; } }

        public OrderSettings settings { get; }
        
        public int orderTime { get { return settings.waitingTime; } }
        public int currentTime { get; private set; }
        public int remainingTime { get { return orderTime - currentTime; } }
        
        public bool isCompleted
        {
            get { return isExpired || _recipes.IsNullOrEmpty(); }
        }

        public bool isExpired
        {
            get { return remainingTime <= 0; }
        }

        public event Action<string> recipeRemoved;

        public OrderClientModel(OrderSettings settings)
        {
            this.settings = settings;

            _recipes = new List<string>(settings.recipesIds);

            currentTime = 0;
        }

        public void Tick()
        {
            if (isCompleted)
                return;
            
            currentTime++;
        }

        public bool TryRemoveRecipe(string id)
        {
            var removed = _recipes.Remove(id);

            if (removed)
            {
                recipeRemoved?.Invoke(id);
            }

            return removed;
        }
    }
}