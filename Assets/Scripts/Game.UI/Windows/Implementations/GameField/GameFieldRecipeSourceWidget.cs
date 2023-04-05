using System;
using Game.Content.Recipes;
using Object = UnityEngine.Object;

namespace Game.UI.Windows
{
    public class GameFieldRecipeSourceWidget : IDisposable
    {
        private readonly GameFieldRecipeSourceWidgetLayout _layout;

        private readonly RecipeEntry _recipeEntry;

        private readonly UISpriteAssigner _spriteAssigner;

        public event Action<RecipeEntry> clicked;

        public GameFieldRecipeSourceWidget(GameFieldRecipeSourceWidgetLayout layout, string recipeId)
        {
            _layout = layout;

            _recipeEntry = ContentManager.GetEntry<RecipeEntry>(recipeId);

            _spriteAssigner = new UISpriteAssigner();
            
            SetupLayout();
        }

        private void SetupLayout()
        {
            _layout.button.onClick.AddListener(HandleClicked);
            
            _spriteAssigner.SetSprite(_recipeEntry.icon, _layout.icons);
        }

        public void Show()
        {
            _layout.rootObject.SetActive(true);
        }

        public void Hide()
        {
            _layout.rootObject.SetActive(false);
        }
        
        public void Dispose()
        {
            _spriteAssigner?.Dispose();
            
            if (_layout == null)
                return;
            
            _layout.button.onClick.RemoveAllListeners();
            
            Object.Destroy(_layout.rootObject);
        }

        private void HandleClicked()
        {
            clicked?.Invoke(_recipeEntry);
        }
    }
}