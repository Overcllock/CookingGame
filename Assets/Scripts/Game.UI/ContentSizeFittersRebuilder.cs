using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ContentSizeFittersRebuilder : MonoBehaviour
{
    [SerializeField]
    private ContentSizeFitter[] _contentSizeFitters;

    [SerializeField]
    private float _autoForceDelay = 0.1f;

    private Action _endAction;

    private void Start()
    {
        if (_autoForceDelay > 0)
            ForceUpdateLayout(_autoForceDelay);
    }

    public void ForceUpdateLayout(float delay, Action endAction)
    {
        _endAction?.Invoke();
        _endAction = endAction;

        Invoke(nameof(ForceUpdateLayoutWithAction), delay);
    }

    public void ForceUpdateLayout(float delay = 0.01f)
    {
        Invoke(nameof(ForceUpdateLayoutInternal), delay);
    }

    public void ForceUpdateLayoutWithAction()
    {
        ForceUpdateLayoutInternal();

        if (gameObject.IsActive())
            StartCoroutine(DelayInvokeEndActionRoutine());
        else
            ForceInvoke();
    }

    private IEnumerator DelayInvokeEndActionRoutine()
    {
        yield return null;

        ForceInvoke();
    }

    private void ForceInvoke()
    {
        _endAction?.Invoke();
        _endAction = null;
    }

    private void ForceUpdateLayoutInternal()
    {
        if(_contentSizeFitters == null) return;
        
        for (var i = 0; i < _contentSizeFitters.Length; i++)
        {
            var csf = _contentSizeFitters[i];

            if (csf == null || csf.gameObject == null) continue;

            csf.enabled = false;
            csf.enabled = true;
            csf.SetLayoutHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate(csf.transform as RectTransform);
            csf.enabled = true;
        }
    }

    [ContextMenu("Force Validate")]
    private void OnValidate()
    {
        _contentSizeFitters = GetComponentsInChildren<ContentSizeFitter>(true);
    }
}