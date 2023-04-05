#if UNITY_EDITOR

using Game.UI.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace Game.Development
{
    public class EditorGUIDropdown : IGUIField
    {
        public Type type { get; private set; }

        private string _label;

        private int _selectedIndex;

        private bool _useFilter;
        private string _filter;
        private string _filterLabel;

        private IGUIDropdownProvider _provider;

        public bool anyValues { get; private set; }
        public bool hasValues { get { return !_provider.GetNames().IsNullOrEmpty(); } }

        public EditorGUIDropdown(Type providerType, string label = null, bool useFilter = true, string filterLabel = null)
        {
            _useFilter = useFilter;
            _filterLabel = filterLabel;
            _label = label;

            _provider = (IGUIDropdownProvider)Activator.CreateInstance(providerType);
            anyValues = !_provider.GetNames().IsNullOrEmpty();
        }

        public void SetSelected(string name)
        {
            _selectedIndex = GetSelectedIndex(name);
        }

        public void Draw()
        {
            if (_useFilter)
            {
                string placeholder =
                    string.IsNullOrEmpty(_filterLabel) ?
                    "Filter..." :
                    _filterLabel;

                _filter = GUIBox.FilterLabel(_filter, placeholder);
                _provider.SetFilter(_filter);
                GUILayout.Space(5);
            }

            var names = _provider.GetNames();

            if (_selectedIndex >= names.Length)
            {
                _selectedIndex = 0;
            }
            
            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };

            _selectedIndex = string.IsNullOrEmpty(_label) ?
                    EditorGUILayout.Popup(_selectedIndex, names, style) :
                    EditorGUILayout.Popup(_label, _selectedIndex, names, style);
        }

        public void ClearFilter()
        {
            _filter = string.Empty;
            _provider.SetFilter(_filter);
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }

        public object GetValue()
        {
            var values = _provider.GetValues();

            if (_selectedIndex >= 0 && _selectedIndex < values.Length)
            {
                return values[_selectedIndex];
            }

            return null;
        }

        private int GetSelectedIndex(string name)
        {
            if (string.IsNullOrEmpty(name)) return 0;

            var names = _provider.GetNames();
            for (int i = 0; i < names.Length; i++)
            {
                if (name == names[i])
                {
                    return i;
                }
            }

            return 0;
        }
    }
}

#endif