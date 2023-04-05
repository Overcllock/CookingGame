using System;
using UnityEngine;

#if UNITY_EDITOR //temp
using UnityEditor;
#endif

namespace Game.Development
{
    public class GUIValueField<T> : GUIValueField
    {
        public GUIValueField() : base(typeof(T))
        {
        }

        new public T GetValue()
        {
            return GetValue<T>();
        }
    }

    public class GUIValueField : ValueField
    {
        public GUIValueField(Type type) : base(type)
        {
        }

        public GUIValueField(Type type, string label) : base(type)
        {
            _label = label + ":";
        }

        protected override void ShowTextField()
        {
            bool showLabel = !string.IsNullOrEmpty(_label);

            if (showLabel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_label, GUILayout.Width(75));
            }

            _stringValue = _style != null ?
                GUILayout.TextField(_stringValue, _style, _options) :
                GUILayout.TextField(_stringValue, _options);

            if (showLabel)
            {
                GUILayout.EndHorizontal();
            }
        }

        protected override void ShowToggle()
        {
            string label = string.IsNullOrEmpty(_label) ? "toggle" : _label;

            _boolValue = _style != null ?
                GUILayout.Toggle(_boolValue, label, _style, _options) :
                GUILayout.Toggle(_boolValue, label, _options);
        }

        protected override void ShowEnum()
        {
#if UNITY_EDITOR //temp
            bool showLabel = !string.IsNullOrEmpty(_label);

            _enumValue = showLabel ?
                EditorGUILayout.EnumPopup(_label, _enumValue, _options) :
                EditorGUILayout.EnumPopup(_enumValue, _options);
#else
            base.ShowEnum();
#endif

        }
    }
}