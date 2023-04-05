using System;
using System.Collections.Generic;
using Game.Content.UI;

namespace Game.UI.Windows
{
    public class UIWindowsManager : IDisposable
    {
        private Dictionary<string, UIWindow> _windowsById =
            new Dictionary<string, UIWindow>();
        private Dictionary<Type, UIWindow> _windowsByType = new Dictionary<Type, UIWindow>();

        public event Action<UIWindow> windowActivated;
        public event Action<UIWindow> windowDeactivated;

        public void Dispose()
        {
            foreach (var window in _windowsById.Values)
            {
                window.openRequested -= HandleWindowOpenRequest;
                window.closeRequested -= HandleWindowCloseRequest;

                window.Dispose();
            }

            _windowsById = null;
            _windowsByType = null;
        }

        public void AddWindow(UIWindow window)
        {
            if (window.id == WindowType.UNDEFINED)
            {
                PostWarning($"Undefined window detected: [{window.GetType().Name}] Please assign valid ID.");
            }

            if (_windowsById.ContainsKey(window.id))
            {
                PostWarning($"Trying to add duplicate window [{window.GetType().Name}]", window.id);
                return;
            }

            _windowsById.Add(window.id, window);
            _windowsByType.Add(window.GetType(), window);

            window.openRequested += HandleWindowOpenRequest;
            window.closeRequested += HandleWindowCloseRequest;
        }

        public void RemoveWindow(UIWindow window)
        {
            if (_windowsById.Remove(window.id))
            {
                _windowsByType.Remove(window.GetType());

                window.Activate(false);

                window.openRequested -= HandleWindowOpenRequest;
                window.closeRequested -= HandleWindowCloseRequest;
            }
            else
            {
                PostWarning($"Trying to remove unstaged window [{window.GetType().Name}]", window.id);
            }
        }

        public bool TryGetWindow(Type type, out UIWindow window)
        {
            return _windowsByType.TryGetValue(type, out window);
        }

        public bool TryGetWindow(string id, out UIWindow window)
        {
            return _windowsById.TryGetValue(id, out window);
        }

        public UIWindow GetWindow(string id)
        {
            if (TryGetWindow(id, out var window))
            {
                return window;
            }

            PostWarning("Window not found!", id);
            return null;
        }

        public void OpenWindow(string id, IWindowArgs args = null)
        {
            if (_windowsById.TryGetValue(id, out var window))
            {
                window.Activate(true, args);
                windowActivated?.Invoke(window);
            }
            else
            {
                PostWarning("Trying to open non-existent window.", id);
            }
        }

        public void CloseAll()
        {
            foreach (var windowId in _windowsById.Keys)
            {
                CloseWindow(windowId);
            }
        }

        public void CloseWindow(string id)
        {
            if (_windowsById.TryGetValue(id, out var window))
            {
                window.Activate(false);
                windowDeactivated?.Invoke(window);
            }
            else
            {
                PostWarning("Trying to close non-existent window.", id);
            }
        }

        public void UpdateWindow(string id, IWindowArgs args = null)
        {
            if (_windowsById.TryGetValue(id, out var window))
            {
                window.UpdateView(args);
            }
        }

        private void HandleWindowOpenRequest(UIWindow window)
        {
            OpenWindow(window.id);
        }

        private void HandleWindowCloseRequest(UIWindow window)
        {
            CloseWindow(window.id);
        }

        private void PostWarning(string message, string windowId = WindowType.UNDEFINED)
        {
            if (windowId != WindowType.UNDEFINED)
            {
                message += " ID: < " + windowId + " >";
            }

            UnityEngine.Debug.LogWarning(message);
        }
    }
}