using Game.Content.UI;
using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.UI.Windows
{
    public abstract class UIWindow<TLayout, TArgs> : UIWindow<TLayout>
        where TLayout : UIBaseWindowLayout
        where TArgs : class, IWindowArgs
    {
        protected sealed override void OnActivate(IWindowArgs args)
        {
            base.OnActivate(args);
            OnActivate(GetNativeArgs(args));
        }

        protected sealed override void OnShow(IWindowArgs args)
        {
            base.OnShow(args);
            OnShow(GetNativeArgs(args));
        }

        protected sealed override void OnUpdate(IWindowArgs args)
        {
            if (_layout != null && args != null && visible)
            {
                OnUpdate(GetNativeArgs(args));
            }
        }

        protected virtual void OnShow(TArgs args)
        {
        }
        
        protected virtual void OnActivate(TArgs args)
        {
        }

        /// <summary>
        /// Called when current view needs to be updated.
        /// Only works if window is already active, visible and has layout created.
        /// </summary>
        protected virtual void OnUpdate(TArgs args)
        {
        }

        private TArgs GetNativeArgs(IWindowArgs args)
        {
            TArgs nativeArgs = args as TArgs;
            if (nativeArgs == null)
            {
                Debug.LogWarning($"Passed invalid args to popup of type: [{GetType()}]");
            }

            return nativeArgs;
        }
    }
    
    public abstract class UIWindow : IDisposable
    {
        protected UIWindowEntry _entry;
        
        protected bool _initialized;

        protected UILayoutFactory _layoutFactory;

        internal event Action<UIWindow> openRequested;
        internal event Action<UIWindow> closeRequested;

        public event Action<UIWindow> activated;
        public event Action<UIWindow> deactivated;
        public event Action<UIWindow> shown;
        public event Action<UIWindow> hidden;
        
        public abstract string id { get; }
        
        public bool active { get; protected set; }
        
        /// <summary>
        /// Is window currently visible?
        /// (as if shown on screen)
        /// </summary>
        public bool visible { get; protected set; }
        
        public virtual void Initialize(UIWindowEntry entry)
        {
            ServiceLocator.Get(out _layoutFactory);

            _entry = entry;

            OnInitialize();
            _initialized = true;
        }
        
        protected virtual void OnInitialize()
        {
        }

        public virtual void Dispose()
        {
        }
        
        public void Activate(bool activate, IWindowArgs args = null)
        {
            if (active != activate)
            {
                active = activate;
                visible = activate;

                if (active)
                {
                    OnActivate(args);

                    activated?.Invoke(this);
                }
                else
                {
                    OnDeactivate();

                    deactivated?.Invoke(this);
                }

                OnVisibilityChanged(visible);
            }
        }

        protected virtual void OnActivate(IWindowArgs args)
        {
        }

        protected virtual void OnDeactivate()
        {
        }

        protected virtual void ShowFader(bool visibility)
        {
        }
        
        public void UpdateView(IWindowArgs args)
        {
            if (!active) 
                return;
            
            OnUpdate(args);
        }
        
        /// <summary>
        /// Called when current view needs to be updated.
        /// Only works if window is already active.
        /// </summary>
        protected virtual void OnUpdate(IWindowArgs args)
        {
        }
        
        /// <summary>
        /// Request window open internally.
        /// </summary>
        protected virtual void RequestOpen()
        {
            if (!active)
            {
                openRequested?.Invoke(this);
            }
        }

        /// <summary>
        /// Request window close internally.
        /// </summary>
        protected virtual void RequestClose()
        {
            closeRequested?.Invoke(this);
        }
        
        protected virtual void OnShow(IWindowArgs args)
        {
            if (_entry.useFader)
            {
                ShowFader(true);
            }
        }

        protected virtual void OnHide()
        {
            if (_entry.useFader)
            {
                ShowFader(false);
            }
        }
        
        protected virtual void OnBeganOpening()
        {
        }
        
        protected virtual void OnEndedOpening()
        {
        }
        
        protected virtual void OnBeganClosing()
        {
        }
        
        protected virtual void OnEndedClosing()
        {
        }
        
        /// <summary>
        /// Called each time visilibity state is changed.
        /// </summary>
        protected virtual void OnVisibilityChanged(bool visible)
        {
        }

        protected virtual void OnLayoutCreated()
        {
        }

        /// <summary>
        /// Called when layout is explicitely destroyed via DestroyLayout method.
        /// Also called if window has auto-destroy option on.
        /// </summary>
        protected virtual void OnLayoutDestroyed()
        {
        }
        
        protected void InvokeShown()
        {
            shown?.Invoke(this);
        }

        protected void InvokeHidden()
        {
            hidden?.Invoke(this);
        }
    }
    
    /// <summary>
    /// This is a simplified UI implementation for test task.
    /// In real projects must be more flexible and MVP-correctly.
    /// </summary>
    public abstract class UIWindow<TLayout> : UIWindow where TLayout : UIBaseWindowLayout
    {
        private Sequence _faderSequence;
        
        protected TLayout _layout;

        public override void Dispose()
        {
            base.Dispose();
            
            _faderSequence?.Kill();
            _faderSequence = null;
            
            DestroyLayout();
        }

        protected override void OnActivate(IWindowArgs args)
        {
            base.OnActivate(args);
            
            if (_layout == null)
            {
                CreateLayout(args);
            }
            else
            {
                OnShow(args);
            }
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            
            _faderSequence?.Kill();
            _faderSequence = null;
            
            if (_layout != null)
            {
                OnHide();
            }
        }

        protected override void ShowFader(bool visibility)
        {
            if (_layout == null)
                return;

            if (visibility)
            {
                _layout.canvasGroup.alpha = 0f;
            }

            _faderSequence = GetFaderSequence(visibility, _entry.faderDuration, _entry.faderDelay);
            _faderSequence.Play();
        }

        private void CreateLayout(IWindowArgs args)
        {
            if (_layout == null)
            {
                _layout = _layoutFactory.Create<TLayout>(_entry.prefabReference);
                _layout.rootObject.SetActive(false);

                OnLayoutCreated();

                OnVisibilityChanged(visible);

                OnShow(args);
            }
        }

        private void DestroyLayout()
        {
            OnLayoutDestroyed();
            
            Object.Destroy(_layout.rootObject);
        }

        private void OpenCallback()
        {
            OnEndedOpening();
            InvokeShown();
        }

        private void CloseCallback()
        {
            OnEndedClosing();
            InvokeHidden();
            DestroyLayout();
        }
        
        private Sequence GetFaderSequence(bool visibility, float duration, float delay)
        {
            var sequence = DOTween.Sequence()
                .Append(_layout.canvasGroup.DOFade(visibility ? 1 : 0, duration))
                .SetEase(Ease.InOutQuint)
                .SetAutoKill(false);

            if (delay > 0)
            {
                sequence.PrependInterval(delay);
            }

            if (visibility)
            {
                sequence.PrependCallback(() => _layout.rootObject.SetActive(true));
            }
            else
            {
                sequence.AppendCallback(() => _layout.rootObject.SetActive(false));
            }

            return sequence;
        }
    }
}