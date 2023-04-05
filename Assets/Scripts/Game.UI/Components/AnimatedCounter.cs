using System.Collections;
using System.Globalization;
using DG.Tweening;
using Game.Content.UI;
using TMPro;
using UnityEngine;

namespace Game.UI.Components
{
    [RequireComponent(typeof(TMP_Text))]
    public class AnimatedCounter : MonoBehaviour
    {
        public float sizeMultiplier = 1.05f;
        public float scalingDuration = 0.3f;
        
        public int fps = 30;
        public float duration = 1f;
        
        public bool isPercentage = false;
        public bool useColoring = true;
        public string postfix;

        private float _value;
        
        private TMP_Text _text;

        private Vector3 _defaultScale;
        private Color _defaultColor;

        private Coroutine _coroutine;
        private Sequence _upSequence;
        private Sequence _downSequence;

        private UIModuleSettings _uiModuleSettings;

        public float value
        {
            get { return _value; }
            set
            {
                UpdateText(value);
                _value = value;
            }
        }

        public TMP_Text text => _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            
            _defaultScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            _defaultColor = _text.color;

            _uiModuleSettings = ContentManager.GetSettings<UIModuleSettings>();
        }

        private void OnDisable()
        {
            Stop();
            
            transform.localScale = _defaultScale;
            _text.color = _defaultColor;
        }

        private void OnDestroy()
        {
            Stop();
        }

        private void Stop()
        {
            SetText(_value);
            
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            _upSequence?.Kill();
            _upSequence = null;

            _downSequence?.Kill();
            _downSequence = null;
        }

        private void UpdateText(float newValue)
        {
            Stop();

            if (gameObject.IsActive())
            {
                _coroutine = StartCoroutine(CountRoutine(newValue));
            }
            else
            {
                SetText(newValue);
            }
        }

        private IEnumerator CountRoutine(float newValue)
        {
            var wait = new WaitForSeconds(1f / fps);

            var prevValue = _value;

            var rawStepValue = (newValue - prevValue) / (fps * duration);
            var stepValue = newValue - prevValue < 0 ? Mathf.FloorToInt(rawStepValue) : Mathf.CeilToInt(rawStepValue);

            if (prevValue < newValue)
            {
                _upSequence = GetUpSequence(true);
                _upSequence.Play();
                
                while (prevValue < newValue)
                {
                    prevValue += stepValue;

                    if (prevValue > newValue)
                    {
                        prevValue = newValue;
                    }
                    
                    SetText(prevValue);

                    yield return wait;
                }
            }
            else if (prevValue > newValue)
            {
                _upSequence = GetUpSequence(false);
                _upSequence.Play();
                
                while (prevValue > newValue)
                {
                    prevValue += stepValue;

                    if (prevValue < newValue)
                    {
                        prevValue = newValue;
                    }
                    
                    SetText(prevValue);

                    yield return wait;
                }
            }
            
            _upSequence?.Kill();
            _upSequence = null;

            _downSequence = GetDownSequence();
            _downSequence.Play();
        }

        private void SetText(float newValue)
        {
            var intVal = Mathf.RoundToInt(newValue);

            string text;
            if (isPercentage)
            {
                text = postfix.IsNullOrEmpty() ? $"{intVal.ToString()}%" : $"{intVal.ToString()}% {postfix}";
            }
            else
            {
                text = intVal != 0 ? intVal.ToString("#,#", CultureInfo.InvariantCulture) : "0";
                if (!postfix.IsNullOrEmpty())
                {
                    text = $"{text} {postfix}";
                }
            }

            _text.text = text;
        }

        private Sequence GetUpSequence(bool positive)
        {
            var sequence = DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(_defaultScale.x * sizeMultiplier, _defaultScale.y * sizeMultiplier, _defaultScale.z * sizeMultiplier), scalingDuration))
                .SetEase(Ease.InOutQuint)
                .SetAutoKill(false);

            if (useColoring)
            {
                sequence.Join(_text.DOColor(positive ? _uiModuleSettings.positiveColor : _uiModuleSettings.negativeColor, scalingDuration));
            }

            return sequence;
        }
        
        private Sequence GetDownSequence()
        {
            var sequence = DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(_defaultScale.x, _defaultScale.y, _defaultScale.z), scalingDuration))
                .SetEase(Ease.InOutQuint)
                .SetAutoKill(false);

            if (useColoring)
            {
                sequence.Join(_text.DOColor(_defaultColor, scalingDuration));
            }
            
            return sequence;
        }
    }
}