using System;
using Game.Campaign;
using Game.UI.Windows;

namespace Game.Presenters
{
    public class CampaignPresenter : IDisposable
    {
        private readonly CampaignService _campaignService;
        private readonly UIWindowsDispatcher _windowsDispatcher;
        
        public CampaignPresenter()
        {
            ServiceLocator.Get(out _campaignService);
            ServiceLocator.Get(out _windowsDispatcher);

            _campaignService.levelFinished += HandleLevelFinished;
        }

        private void HandleLevelFinished(LevelClientModel level)
        {
            var args = new EndGameWindowArgs
            {
                isWin = level.isWin
            };
            
            _windowsDispatcher.ShowWindow<EndGameWindow>(args);
        }
        
        public void Dispose()
        {
            _campaignService.levelFinished -= HandleLevelFinished;
        }
    }
}