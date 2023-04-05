using Game.UI.Utilities;
using System;
using UnityEngine;

namespace Game.Development
{
    public class GUIDropdownField : IGUIField
    {
        public Type type { get; private set; }

        private string _label;

        private string[] _options;
        private object[] _values;

        private int _selectedIndex;

        private bool _open;
        private Vector2 _scrollPos;

        private IGUIDropdownProvider _provider;

        public GUIDropdownField(Type type, Type providerType, string label = null)
        {
            _provider = (IGUIDropdownProvider)Activator.CreateInstance(providerType);
            Init(type, label);
        }

        private void Init(Type type, string label)
        {
            this.type = type;

            if (!string.IsNullOrEmpty(label))
            {
                _label = label + ":";
            }

            Update();
        }

        private void Update()
        {
            _options = _provider.GetNames();
            _values = _provider.GetValues();
        }

        public void Draw()
        {
            if (_provider.autoUpdate)
            {
                Update();
            }
            
            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            
            bool showLabel = !_open && !string.IsNullOrWhiteSpace(_label);

            if (showLabel)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(_label, style, GUILayout.Width(75));
            }

            _selectedIndex = GUIBox.Dropdown(_selectedIndex, _options, ref _open, ref _scrollPos, style, GUILayout.Height(150));

            if (showLabel)
            {
                GUILayout.EndHorizontal();
            }
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        public object GetValue()
        {
            if (_selectedIndex >= 0 && _selectedIndex < _values.Length)
            {
                return _values[_selectedIndex];
            }

            return null;
        }
    }
}