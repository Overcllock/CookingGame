using Game.Content.UI;
using System;
using Game.Utilities;

namespace Game.UI.Windows
{
    public class UIWindowsDispatcher
    {
        private readonly UIWindowsManager _manager;

        public UIWindowsDispatcher(UIWindowsManager manager)
        {
            _manager = manager;
        }

        public void PreloadWindow<TWindow>() where TWindow : UIWindow
        {
            PrepareWindow<TWindow>();
        }

        public void ShowWindow<TWindow>() where TWindow : UIWindow
        {
            var window = PrepareWindow<TWindow>();
            _manager.OpenWindow(window.id);
        }
        
        public void ShowWindow<T>(IWindowArgs args) where T : UIWindow
        {
            Precondition.ArgumentMustNotBeNull(args, "args",
                $"Could not open window of type: [{typeof(T)}]");

            var window = PrepareWindow<T>();
            _manager.OpenWindow(window.id, args);
        }

        public bool IsWindowShowed<T>() where T : UIWindow
        {
            if (_manager.TryGetWindow(typeof(T), out var window))
            {
                if (window.active && window.visible)
                    return true;
            }

            return false;
        }

        public void HideWindow<TWindow>() where TWindow : UIWindow
        {
            if (_manager.TryGetWindow(typeof(TWindow), out var window))
            {
                _manager.CloseWindow(window.id);
            }
        }

        public void HideAll()
        {
            _manager.CloseAll();
        }

        public void UpdateWindow<TWindow>(IWindowArgs args = null) where TWindow : UIWindow
        {
            if (_manager.TryGetWindow(typeof(TWindow), out var window))
            {
                _manager.UpdateWindow(window.id, args);
            }
        }

        private UIWindow PrepareWindow<TWindow>() where TWindow : UIWindow
        {
            var type = typeof(TWindow);
            if (!_manager.TryGetWindow(type, out var window))
            {
                window = Activator.CreateInstance(type) as TWindow;

                if (window == null)
                {
                    throw new Exception($"Could not configure window: [{type}]. Invalid type");
                }

                if (!ContentManager.TryGetEntry(window.id, out UIWindowEntry entry))
                {
                    throw new Exception($"Could not configure window: [{type}][{window.id}]. Check GUI database");
                }

                window.Initialize(entry);
                _manager.AddWindow(window);
            }

            return window;
        }
    }
    
    public interface IWindowArgs { }
}