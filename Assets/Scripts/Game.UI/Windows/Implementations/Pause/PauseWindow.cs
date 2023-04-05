using Game.Campaign;
using Game.Content.Campaign;
using Game.Content.UI;

namespace Game.UI.Windows
{
    public class PauseWindow : UIWindow<PauseWindowLayout, PauseWindowArgs>
    {
        public override string id => WindowType.PAUSE_WINDOW;
        
        private CampaignService _campaignService;
        private GameLoopService _gameLoopService;
        
        protected override void OnInitialize()
        {
            ServiceLocator.Get(out _campaignService);
            ServiceLocator.Get(out _gameLoopService);
        }
        
        protected override void OnShow(PauseWindowArgs args)
        {
            base.OnShow(args);
            
            if (_layout != null)
            {
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(true);
                }
                
                _layout.continueButton.button.onClick.AddListener(HandleContinueClick);
                _layout.exitButton.button.onClick.AddListener(HandleQuitClick);
                _layout.restartButton.button.onClick.AddListener(HandleRestartClick);
            }
        }

        protected override void OnHide()
        {
            base.OnHide();
            
            if (_layout != null)
            {
                _layout.continueButton.button.onClick.RemoveAllListeners();
                _layout.exitButton.button.onClick.RemoveAllListeners();
                _layout.restartButton.button.onClick.RemoveAllListeners();
                
                if (!_entry.useFader)
                {
                    _layout.rootObject.SetActive(false);
                }
            }
        }

        private void HandleContinueClick()
        {
            RequestClose();
        }
        
        private void HandleQuitClick()
        {
            _gameLoopService.TrySwitchTo(GameMode.TitleScreen);
            
            RequestClose();
        }
        
        private void HandleRestartClick()
        {
            var levelEntry = ContentManager.GetEntry<LevelEntry>("Demo");
            
            _campaignService.Reset();
            _campaignService.StartLevel(levelEntry);
            
            RequestClose();
        }
    }

    public class PauseWindowArgs : IWindowArgs
    {
    }
}