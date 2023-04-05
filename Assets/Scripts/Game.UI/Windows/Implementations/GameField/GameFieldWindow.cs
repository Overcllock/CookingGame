using System.Collections.Generic;
using Game.Campaign;
using Game.Content.Recipes;
using Game.Content.UI;

namespace Game.UI.Windows
{
    public class GameFieldWindow : UIWindow<GameFieldWindowLayout, GameFieldWindowArgs>
    {
        public override string id => WindowType.GAME_FIELD;
        
        private CampaignService _campaignService;

        private readonly Dictionary<string, GameFieldRecipeSourceWidget> _recipeSourceWidgets =
            new Dictionary<string, GameFieldRecipeSourceWidget>();

        private readonly Dictionary<int, GameFieldGuestWidget> _guestWidgets = 
            new Dictionary<int, GameFieldGuestWidget>();

        protected override void OnInitialize()
        {
            ServiceLocator.Get(out _campaignService);
        }

        protected override void OnShow(GameFieldWindowArgs args)
        {
            base.OnShow(args);
            
            _campaignService.levelOrderCreated += HandleLevelOrderCreated;
            _campaignService.levelOrderRemoved += HandleLevelOrderRemoved;
            _campaignService.levelStarted += HandleLevelStarted;
            
            if (_layout != null)
            {
                UpdateRecipeSourceWidgets();
                
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(true);
                }
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            OnDispose();
            
            if (_layout != null)
            {
                
                
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(false);
                }
            }
        }

        private void OnDispose()
        {
            _campaignService.levelOrderCreated -= HandleLevelOrderCreated;
            _campaignService.levelOrderRemoved -= HandleLevelOrderRemoved;
            _campaignService.levelStarted -= HandleLevelStarted;
            
            foreach (var widget in _recipeSourceWidgets.Values)
            {
                widget.clicked -= HandleRecipeSourceClicked;
                widget.Dispose();
            }

            foreach (var widget in _guestWidgets.Values)
            {
                widget?.Dispose();
            }
            
            _recipeSourceWidgets.Clear();
            _guestWidgets.Clear();
        }

        private void UpdateRecipeSourceWidgets()
        {
            if (_layout == null || _campaignService.currentLevel == null)
                return;
            
            foreach (var widget in _recipeSourceWidgets.Values)
            {
                widget.clicked -= HandleRecipeSourceClicked;
                widget.Dispose();
            }
            
            _recipeSourceWidgets.Clear();

            var levelEntry = _campaignService.currentLevel.entry;
            
            if (levelEntry.recipesSources.IsNullOrEmpty())
                return;

            for (int i = 0; i < levelEntry.recipesSources.Length; i++)
            {
                var recipeId = levelEntry.recipesSources[i];
                
                var layout = _layoutFactory.Create(_layout.recipeSourceWidgetTemplate, _layout.recipeSourceWidgetTemplate.transform.parent);
                
                var widget = new GameFieldRecipeSourceWidget(layout, recipeId);
                widget.clicked += HandleRecipeSourceClicked;
                
                widget.Show();
                
                _recipeSourceWidgets.Add(recipeId, widget);
            }
        }

        private void HandleRecipeSourceClicked(RecipeEntry recipeEntry)
        {
            if (_campaignService.currentLevel == null)
                return;

            var count = _campaignService.currentLevel.entry.ordersCapacity;
            for (int i = 0; i < count; i++)
            {
                if (_campaignService.TryRemoveOrderRecipe(i, recipeEntry.id))
                    break;
            }
        }

        private void HandleLevelStarted(LevelClientModel level)
        {
            foreach (var widget in _guestWidgets.Values)
            {
                widget?.Dispose();
            }
            
            _guestWidgets.Clear();
            
            UpdateRecipeSourceWidgets();
        }

        private void HandleLevelOrderCreated(int index, OrderClientModel order)
        {
            var layout = _layoutFactory.Create(_layout.guestWidgetTemplate, _layout.guestWidgetTemplate.transform.parent);
                
            var widget = new GameFieldGuestWidget(layout, index, order);
            widget.Show();

            _guestWidgets[index] = widget;
        }
        
        private void HandleLevelOrderRemoved(int index, OrderClientModel order)
        {
            if (_guestWidgets.TryGetValue(index, out var widget))
            {
                widget?.Hide(dispose: true);
                _guestWidgets.Remove(index);
            }
        }
    }
    
    public class GameFieldWindowArgs : IWindowArgs
    {
    }
}