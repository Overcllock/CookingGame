using System;
using UnityEngine;

namespace Game.Development
{
    public abstract class ValueField : IGUIField
    {
        protected string _label;
        protected string _stringValue;
        protected bool _boolValue;
        protected Enum _enumValue;

        protected GUIStyle _style;
        protected GUILayoutOption[] _options;

        public Type type { get; protected set; }
        public bool enabled { get; set; } = true;

        public ValueField(Type type)
        {
            this.type = type;

            if (type.IsEnum)
            {
                _enumValue = (Enum)Activator.CreateInstance(type);
            }
        }

        public ValueField(Type type, string label)
        {
            this.type = type;
            _label = label;

            if (type.IsEnum)
            {
                _enumValue = (Enum)Activator.CreateInstance(type);
            }
        }

        public virtual void Draw()
        {
            bool guiEnabled = GUI.enabled;
            GUI.enabled = enabled;

            if (type == typeof(bool))
            {
                ShowToggle();
            }
            else
            if (type.IsEnum)
            {
                ShowEnum();
            }
            else
            {
                ShowTextField();
            }

            GUI.enabled = guiEnabled;
        }

        public void SetStyle(GUIStyle style, params GUILayoutOption[] options)
        {
            _style = style;
            SetOptions(options);
        }

        public void SetOptions(params GUILayoutOption[] options)
        {
            _options = options;
        }

        protected abstract void ShowTextField();
        protected abstract void ShowToggle();
        protected virtual void ShowEnum()
        {
            ShowTextField(); //temp
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        public object GetValue()
        {
            if (type == typeof(string)) return _stringValue;

            if (type == typeof(bool))
            {
                return _boolValue;
            }

            if (type.IsEnum)
            {
                return _enumValue;
            }

            if (string.IsNullOrEmpty(_stringValue)) return default;

            if (type == typeof(int))
            {
                if (int.TryParse(_stringValue, out int result))
                {
                    return result;
                }
            }

            if (type == typeof(long))
            {
                if (long.TryParse(_stringValue, out long result))
                {
                    return result;
                }
            }

            if (type == typeof(double))
            {
                if (double.TryParse(_stringValue, out double result))
                {
                    return result;
                }
            }

            if (type == typeof(float))
            {
                if (float.TryParse(_stringValue, out float result))
                {
                    return result;
                }
            }

            if (type == typeof(Vector3))
            {
                if (Vector3Utility.TryParse(_stringValue, out Vector3 vector))
                {
                    return vector;
                }
            }

            Debug.LogWarning($"Unsupported type for GUIValueField: [{type}]");
            return default;
        }
    }
}