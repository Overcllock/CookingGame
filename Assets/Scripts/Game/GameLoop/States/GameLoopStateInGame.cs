using Game.Campaign;
using Game.Content.Campaign;
using Game.UI.Windows;

namespace Game
{
    public class GameLoopStateInGame : GameLoopState
    {
        public override GameMode GetMode() => GameMode.InGame;

        private readonly UIWindowsDispatcher _windowsDispatcher;
        private readonly CampaignService _campaignService;

        public GameLoopStateInGame()
        {
            ServiceLocator.Get(out _windowsDispatcher);
            ServiceLocator.Get(out _campaignService);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _campaignService.Reset();
            
            _windowsDispatcher.ShowWindow<GameFieldWindow>();
            _windowsDispatcher.ShowWindow<HUD>();

            var levelEntry = ContentManager.GetEntry<LevelEntry>("Demo");
            _campaignService.StartLevel(levelEntry);
        }
        
        public override void OnExit()
        {
            base.OnExit();

            _windowsDispatcher.HideWindow<GameFieldWindow>();
            _windowsDispatcher.HideWindow<HUD>();
            _windowsDispatcher.HideWindow<PauseWindow>();
        }
    }
}