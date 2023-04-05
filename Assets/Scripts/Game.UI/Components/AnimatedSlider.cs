using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Components
{
    [RequireComponent(typeof(Slider))]
    public class AnimatedSlider : MonoBehaviour
    {
        public float duration = 1f;

        public float value
        {
            get
            {
                return _slider.value;
            }
            set
            {
                SetValue(value);
            }
        }

        private Slider _slider;

        private Tween _moveTween;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        private void OnDisable()
        {
            KillTween();
        }

        private void OnDestroy()
        {
            KillTween();
        }

        private void KillTween()
        {
            _moveTween?.Kill();
            _moveTween = null;
        }

        private void SetValue(float newValue)
        {
            KillTween();

            if (gameObject.IsActive())
            {
                _moveTween = _slider.DOValue(newValue, duration);
                _moveTween.Play();
            }
            else
            {
                _slider.value = newValue;
            }
        }
    }
}