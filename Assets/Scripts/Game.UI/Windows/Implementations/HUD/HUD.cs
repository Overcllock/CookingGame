using Game.Campaign;
using Game.Content.UI;

namespace Game.UI.Windows
{
    public class HUD : UIWindow<HUDLayout, HUDArgs>
    {
        public override string id => WindowType.HUD;

        private CampaignService _campaignService;
        
        private UIWindowsDispatcher _windowsDispatcher;
        
        protected override void OnInitialize()
        {
            ServiceLocator.Get(out _campaignService);
            ServiceLocator.Get(out _windowsDispatcher);
        }

        protected override void OnShow(HUDArgs args)
        {
            base.OnShow(args);

            _campaignService.levelTimeChanged += HandleLevelTimeChanged;
            _campaignService.levelOrderCreated += HandleLevelOrderCreated;
            _campaignService.levelOrderRemoved += HandleLevelOrderRemoved;
            _campaignService.levelStarted += HandleLevelStarted;
            _campaignService.levelFinished += HandleLevelFinished;
            
            if (_layout != null)
            {
                _layout.pauseButton.onClick.AddListener(HandlePauseClick);

                UpdateLayout();
                
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(true);
                }
            }
        }

        protected override void OnHide()
        {
            base.OnHide();

            _campaignService.levelTimeChanged -= HandleLevelTimeChanged;
            _campaignService.levelOrderCreated -= HandleLevelOrderCreated;
            _campaignService.levelOrderRemoved -= HandleLevelOrderRemoved;
            _campaignService.levelStarted -= HandleLevelStarted;
            _campaignService.levelFinished -= HandleLevelFinished;

            if (_layout != null)
            {
                _layout.pauseButton.onClick.RemoveAllListeners();
                
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(false);
                }
            }
        }
        
        private void UpdateLayout()
        {
            if (_layout == null || _campaignService.currentLevel == null)
                return;
            
            UpdateTimer();

            var overallGuestsCount = _campaignService.currentLevel.ordersCount;
            var currentGuestsCount = _campaignService.currentLevel.completedOrdersCount;
            var remainingGuestsCount = _campaignService.currentLevel.remainingOrdersCount;

            _layout.overallGuestsLabel.text = remainingGuestsCount.ToString();
            _layout.currentGuestsLabel.text = $"{currentGuestsCount.ToString()}/{overallGuestsCount.ToString()}";

            _layout.currentGuestsSlider.value = currentGuestsCount / (float) overallGuestsCount;
        }

        private void UpdateTimer()
        {
            if (_layout == null || _campaignService.currentLevel == null)
                return;
            
            _layout.timeLabel.text = _campaignService.currentLevel.remainingTime.ToString();
        }

        private void HandleLevelTimeChanged(LevelClientModel level)
        {
            UpdateTimer();
        }
        
        private void HandleLevelStarted(LevelClientModel level)
        {
            UpdateLayout();
        }
        
        private void HandleLevelFinished(LevelClientModel level)
        {
            UpdateLayout();
        }
        
        private void HandleLevelOrderCreated(int index, OrderClientModel order)
        {
            UpdateLayout();
        }
        
        private void HandleLevelOrderRemoved(int index, OrderClientModel order)
        {
            UpdateLayout();
        }

        private void HandlePauseClick()
        {
            _windowsDispatcher.ShowWindow<PauseWindow>();
        }
    }

    public class HUDArgs : IWindowArgs
    {
    }
}