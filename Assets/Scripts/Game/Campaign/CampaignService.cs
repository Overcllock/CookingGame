using System;
using Game.Content.Campaign;
using Game.Events;

namespace Game.Campaign
{
    public class CampaignService : IDisposable
    {
        public LevelClientModel currentLevel { get; private set; }

        public event Action<LevelClientModel> levelStarted;
        public event Action<LevelClientModel> levelFinished;

        public event Action<LevelClientModel> levelTimeChanged;
        
        public event Action<int, OrderClientModel> levelOrderCreated;
        public event Action<int, OrderClientModel> levelOrderRemoved;

        public CampaignService()
        {
            EngineEvents.Subscribe(EventsType.EachSecond, HandleEachSecond);
        }

        public bool CanStartLevel(LevelEntry levelEntry)
        {
            return currentLevel == null;
        }

        public void StartLevel(LevelEntry levelEntry)
        {
            if (!CanStartLevel(levelEntry))
                return;

            var level = LevelDataFactory.CreateLevel(levelEntry);

            level.timeChanged += HandleLevelTimeChanged;
            level.orderCreated += HandleLevelOrderCreated;
            level.orderRemoved += HandleLevelOrderRemoved;
            
            currentLevel = level;
            
            levelStarted?.Invoke(level);
        }

        public void FinishCurrentLevel()
        {
            if (currentLevel != null)
            {
                levelFinished?.Invoke(currentLevel);
            }
            
            Reset();
        }

        public bool TryRemoveOrderRecipe(int orderIndex, string recipeId)
        {
            if (currentLevel == null)
                return false;

            if (!currentLevel.TryRemoveOrderRecipe(orderIndex, recipeId))
                return false;
            
            currentLevel.TryRemoveOrder(orderIndex);
            return true;
        }
        
        public void Reset()
        {
            if (currentLevel != null)
            {
                currentLevel.timeChanged -= HandleLevelTimeChanged;
                currentLevel.orderCreated -= HandleLevelOrderCreated;
                currentLevel.orderRemoved -= HandleLevelOrderRemoved;

                currentLevel = null;
            }
        }
        
        public void Dispose()
        {
            EngineEvents.Unsubscribe(EventsType.EachSecond, HandleEachSecond);
            
            Reset();
        }

        private void HandleEachSecond()
        {
            if (currentLevel == null)
                return;
            
            currentLevel.Tick();

            if (currentLevel.isCompleted)
            {
                FinishCurrentLevel();
                return;
            }

            for (int i = 0; currentLevel != null && i < currentLevel.currentOrders.Count; i++)
            {
                currentLevel.TryRemoveOrder(i);
            }

            currentLevel?.TryCreateOrder();
        }

        private void HandleLevelTimeChanged(LevelClientModel level)
        {
            levelTimeChanged?.Invoke(level);
        }
        
        private void HandleLevelOrderCreated(int index, OrderClientModel order)
        {
            levelOrderCreated?.Invoke(index, order);
        }

        private void HandleLevelOrderRemoved(int index, OrderClientModel order)
        {
            levelOrderRemoved?.Invoke(index, order);
            
            if (currentLevel.isCompleted)
            {
                FinishCurrentLevel();
                return;
            }

            currentLevel.TryCreateOrder();
        }
    }
}