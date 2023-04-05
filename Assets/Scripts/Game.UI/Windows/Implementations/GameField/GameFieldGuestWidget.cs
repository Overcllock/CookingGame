using System;
using DG.Tweening;
using Game.Campaign;
using Game.Content.Guests;
using Game.Content.Recipes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI.Windows
{
    public class GameFieldGuestWidget : IDisposable
    {
        private const int POS_OFFSET = 21;
        
        private readonly GameFieldGuestWidgetLayout _layout;

        private readonly int _index;
        private readonly OrderClientModel _order;

        private readonly UISpriteAssigner _guestIconSpriteAssigner;
        private readonly UISpriteAssigner[] _recipeIconsSpriteAssigners;

        private Sequence _sliderSequence;
        private Sequence _showSequence;
        private Sequence _hideSequence;

        private Vector3 _offscreenPosLeft;
        private Vector3 _offscreenPosRight;
        private Vector3 _pos;

        public GameFieldGuestWidget(GameFieldGuestWidgetLayout layout, int index, OrderClientModel order)
        {
            _layout = layout;
            _index = index;
            _order = order;

            _guestIconSpriteAssigner = new UISpriteAssigner();
            _recipeIconsSpriteAssigners = new UISpriteAssigner[_layout.recipesIcons.Length];
            
            SetupLayout();

            _order.recipeRemoved += HandleRecipeRemoved;
        }

        private void SetupLayout()
        {
            var defaultPos = _layout.rootRect.localPosition;
            _offscreenPosLeft = new Vector3(defaultPos.x - POS_OFFSET - _layout.rootRect.rect.width, defaultPos.y, defaultPos.z);

            var parentRect = _layout.rootObject.transform.parent as RectTransform;
            _offscreenPosRight = new Vector3(defaultPos.x + POS_OFFSET + parentRect.rect.width, defaultPos.y, defaultPos.z);

            var offset = _index == 0 ? POS_OFFSET : (POS_OFFSET + _layout.rootRect.rect.width) * _index;
            _pos = new Vector3(defaultPos.x + offset, defaultPos.y, defaultPos.z);
            
            var guestEntry = ContentManager.GetEntry<GuestEntry>(_order.settings.guestId);
            
            _guestIconSpriteAssigner.SetSprite(guestEntry.icon, _layout.icon);

            UpdateRecipes();
        }

        private void UpdateRecipes()
        {
            for (int i = 0; i < _layout.recipesIcons.Length; i++)
            {
                if (_order.recipes.Count <= i)
                {
                    _layout.recipesIcons[i].SetActive(false);
                    continue;
                }
                
                var recipeEntry = ContentManager.GetEntry<RecipeEntry>(_order.recipes[i]);

                var assigner = _recipeIconsSpriteAssigners[i];
                if (assigner == null)
                {
                    assigner = new UISpriteAssigner();
                }
                
                assigner.SetSprite(recipeEntry.icon, _layout.recipesIcons[i]);
            }
        }

        public void Show()
        {
            KillSequences();
            
            _layout.rootObject.SetActive(true);
            
            _sliderSequence = GetSliderSequence(_order.orderTime);
            _sliderSequence.Play();

            _showSequence = GetShowSequence();
            _showSequence.Play();
        }

        public void Hide(bool dispose = false)
        {
            KillSequences();

            _hideSequence = GetHideSequence(dispose);
            _hideSequence.Play();
        }

        private void KillSequences()
        {
            _sliderSequence?.Kill();
            _sliderSequence = null;
            
            _showSequence?.Kill();
            _showSequence = null;

            _hideSequence?.Kill();
            _hideSequence = null;
        }
        
        public void Dispose()
        {
            KillSequences();
            
            _order.recipeRemoved -= HandleRecipeRemoved;

            _guestIconSpriteAssigner?.Dispose();

            foreach (var assigner in _recipeIconsSpriteAssigners)
            {
                assigner?.Dispose();
            }

            if (_layout != null)
            {
                Object.Destroy(_layout.rootObject);
            }
        }

        private void HandleRecipeRemoved(string id)
        {
            if (_layout == null || _order == null)
                return;

            UpdateRecipes();
        }

        private Sequence GetSliderSequence(int time)
        {
            return DOTween.Sequence()
                .Append(_layout.timeSlider.DOValue(0f, time).From(1f))
                .SetEase(Ease.Linear)
                .SetAutoKill(false);
        }
        
        private Sequence GetShowSequence()
        {
            const float TIME = 1f;
            
            return DOTween.Sequence()
                .Append(_layout.rootRect.DOLocalMove(_pos, TIME).From(_offscreenPosRight))
                .SetEase(Ease.InOutQuint)
                .SetAutoKill(false);
        }
        
        private Sequence GetHideSequence(bool dispose = false)
        {
            const float TIME = 1f;
            
            var sequence = DOTween.Sequence()
                .Append(_layout.rootRect.DOLocalMove(_offscreenPosLeft, TIME))
                .SetEase(Ease.InOutQuint)
                .SetAutoKill(false);

            if (dispose)
            {
                sequence.AppendCallback(Dispose);
            }

            return sequence;
        }
    }
}