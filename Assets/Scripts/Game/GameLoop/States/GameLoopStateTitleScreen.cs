using Game.UI.Windows;

namespace Game
{
    public class GameLoopStateTitleScreen : GameLoopState
    {
        public override GameMode GetMode() => GameMode.TitleScreen;

        private readonly UIWindowsDispatcher _windowsDispatcher;

        public GameLoopStateTitleScreen()
        {
            ServiceLocator.Get(out _windowsDispatcher);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _windowsDispatcher.ShowWindow<MainMenuWindow>();
        }
    }
}