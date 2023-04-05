using Game.Development;
using Game.Utilities;
using System;
using UnityEngine;

namespace UnityEditor
{
    public class EditorGUIField : ValueField
    {
        private float _labelWidth;
        public string label { get { return _label; } }

        public EditorGUIField(Type type, string label,
            bool enabled = true,
            float labelWidth = 75f,
            GUIStyle style = null,
            params GUILayoutOption[] options) : base(type, label)
        {
            this.enabled = enabled;
            _label = label;

            _labelWidth = labelWidth;
            SetStyle(style, options);
        }

        public EditorGUIField(object value, string label,
            bool enabled = true,
            float labelWidth = 75f,
            GUIStyle style = null,
            params GUILayoutOption[] options) : base(value.GetDeclaredType())
        {
            this.enabled = enabled;

            _stringValue = value != null ?
                value.ToString() :
                "unknown type";

            _boolValue = type == typeof(bool) ?
                (bool)value :
                false;

            _label = label;

            _labelWidth = labelWidth;
            SetStyle(style, options);
        }

        protected override void ShowTextField()
        {
            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = _labelWidth;

            _stringValue = _style != null ?
                    EditorGUILayout.TextField(_label, _stringValue, _style, _options) :
                    EditorGUILayout.TextField(_label, _stringValue, _options);

            EditorGUIUtility.labelWidth = defaultWidth;
        }

        protected override void ShowToggle()
        {
            float defaultWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = _labelWidth;

            _boolValue = _style != null ?
                EditorGUILayout.Toggle(_label, _boolValue, _style) :
                EditorGUILayout.Toggle(_label, _boolValue);

            EditorGUIUtility.labelWidth = defaultWidth;
        }
    }
}