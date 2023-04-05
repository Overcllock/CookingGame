#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Game.Development
{
    public class GUIToggleBox
    {
        private enum ViewType
        {
            Basic,
            Titled,
            LiteralSwitch,
            ContentSwitch
        }

        private bool _lastValue;
        private string _editorPrefsKey;

        private string _title;

        private string _enabledState;
        private string _disabledState;
        private GUIContent _enabledContent;
        private GUIContent _disabledContent;

        private ViewType _type;

        private Action _switchAction;

        public bool Value { get; private set; }

        public GUIToggleBox(string editorPrefsKey)
        {
            _type = ViewType.Basic;

            TryLoadValue(editorPrefsKey);
        }

        public GUIToggleBox(string title, string editorPrefsKey)
        {
            _type = ViewType.Titled;

            _title = title;
            TryLoadValue(editorPrefsKey);
        }

        public GUIToggleBox(string enabled, string disabled, string editorPrefsKey)
        {
            _type = ViewType.LiteralSwitch;

            _enabledState = enabled;
            _disabledState = disabled;

            TryLoadValue(editorPrefsKey);
        }

        public GUIToggleBox(GUIContent enabled, GUIContent disabled, string editorPrefsKey)
        {
            _type = ViewType.ContentSwitch;

            _enabledContent = enabled;
            _disabledContent = disabled;

            TryLoadValue(editorPrefsKey);
        }

        private void TryLoadValue(string editorPrefsKey)
        {
            _editorPrefsKey = editorPrefsKey;
            if (EditorPrefs.HasKey(_editorPrefsKey))
            {
                Value = EditorPrefs.GetBool(_editorPrefsKey);
                _lastValue = Value;
            }
        }

        public void BindSwitchAction(Action action)
        {
            _switchAction = action;
        }

        public void Draw(params GUILayoutOption[] options)
        {
            switch (_type)
            {
                case ViewType.Basic:
                    Value = EditorGUILayout.Toggle(Value, options);
                    break;
                case ViewType.Titled:
                    Value = GUILayout.Toggle(Value, _title, options);
                    break;
                case ViewType.LiteralSwitch:
                    DrawLiteralSwitch();
                    break;
                case ViewType.ContentSwitch:
                    DrawContentSwitch();
                    break;
            }

            CheckValue();
        }

        public void Draw(GUIStyle style, params GUILayoutOption[] options)
        {
            switch (_type)
            {
                case ViewType.Basic:
                    Value = EditorGUILayout.Toggle(Value, style, options);
                    break;
                case ViewType.Titled:
                    Value = GUILayout.Toggle(Value, _title, options);
                    break;
                case ViewType.LiteralSwitch:
                    DrawLiteralSwitch(style);
                    break;
                case ViewType.ContentSwitch:
                    DrawContentSwitch(style);
                    break;
            }

            CheckValue();
        }

        public void DrawLiteralSwitch(GUIStyle style = null)
        {
            string label = Value ? _enabledState : _disabledState;

            if (style == null ?
                GUILayout.Button(label, GUI.skin.label, GUILayout.Width(30)) :
                GUILayout.Button(label, style, GUILayout.Width(30)))
            {
                Value = !Value;
            }
        }

        public void DrawContentSwitch(GUIStyle style = null)
        {
            GUIContent content = Value ? _enabledContent : _disabledContent;

            if (style == null ?
                GUILayout.Button(content, GUI.skin.label, GUILayout.Width(30), GUILayout.Height(18)) :
                GUILayout.Button(content, style, GUILayout.Width(30), GUILayout.Height(18)))
            {
                Value = !Value;
            }
        }

        private void CheckValue()
        {
            if (Value != _lastValue)
            {
                _lastValue = Value;
                EditorPrefs.SetBool(_editorPrefsKey, _lastValue);

                _switchAction?.Invoke();
            }
        }

        public void Clear()
        {
            if (EditorPrefs.HasKey(_editorPrefsKey))
            {
                EditorPrefs.DeleteKey(_editorPrefsKey);
            }
        }
    }
}
#endif