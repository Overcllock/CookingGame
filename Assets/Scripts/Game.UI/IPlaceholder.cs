using System;

namespace Game.UI
{
    public interface IPlaceholder : IDisposable
    {
        public void Show();
        public void Hide();
    }
}