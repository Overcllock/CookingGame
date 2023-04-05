using Game.UI.Utilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor
{
    public class GUIPopupSelector<T> : OdinSelector<T> where T : class
    {
        private bool _show;
        private OdinEditorWindow _window;

        private readonly T[] _values;

        private Func<T, string> _pathEvaluator;
        private Action<T> _onSelected;

        private bool _enableSearch;
        private bool _enableMultiselect;
        private OdinMenuTree _tree;

        private Dictionary<T, string> _cacheItemNames;
        private List<FunctionButtonInfo> _functionButtonsInfo;

        private Func<OdinMenuItem, bool> _searchFunction;

        public T selectedValue { get; private set; }

        public GUIPopupSelector(T[] values, T selectedValue, Action<T> onSelected,
            Func<T, string> pathEvaluator = null,
            Action onRefresh = null,
            bool search = true,
            bool multiselect = false)
        {
            _values = values;
            _onSelected = onSelected;
            _pathEvaluator = pathEvaluator;

            _enableSearch = search;
            _enableMultiselect = multiselect;
            _searchFunction = DefaultSearchFunction;

            if (onRefresh != null)
            {
                _functionButtonsInfo = new List<FunctionButtonInfo>
                {
                    new FunctionButtonInfo
                    {
                        action = onRefresh,
                        icon = EditorIcons.Refresh
                    }
                };
            }

            this.selectedValue = ValidateSelectedValue(selectedValue) ?
                selectedValue :
                null;
        }

        public void Hide()
        {
            Hide(true);
        }

        public void Hide(bool close)
        {
            if (close)
            {
                TryClose();
            }

            _show = false;
        }

        public void SetSearchFunction(Func<OdinMenuItem, bool> function)
        {
            _searchFunction = function;

            if (_tree != null)
            {
                _tree.Config.SearchFunction = function;
            }
        }

        public string GetSearchTerm()
        {
            return _tree?.Config.SearchTerm ?? string.Empty;
        }

        public bool Contains(T value)
        {
            return 
                _values != null ? 
                _values.Contains(value) : 
                false;
        }

        public void AddFunctionButtons(params FunctionButtonInfo[] infos)
        {
            if (infos.IsNullOrEmpty()) return;

            if (_functionButtonsInfo == null)
            {
                _functionButtonsInfo = new List<FunctionButtonInfo>();
            }

            for (int i = infos.Length; i-- > 0;)
            {
                _functionButtonsInfo.Add(infos[i]);
            }
        }

        public void Draw(Rect position)
        {
            var label = selectedValue != null ?
                selectedValue.ToString() :
                string.Empty;

            Draw(position, new GUIContent(label));
        }

        public void Draw(Rect position, GUIContent valueLabel)
        {
            if (!_functionButtonsInfo.IsNullOrEmpty())
            {
                position.width -= (position.height * _functionButtonsInfo.Count);
            }

            DrawDropdown(position, valueLabel);
            DrawFunctionButtons(position);

            UpdateItemsNameOnSearch();
        }

        private void DrawDropdown(Rect position, GUIContent valueLabel)
        {
            if (EditorGUI.DropdownButton(position, valueLabel, FocusType.Passive))
            {
                _show = !_show;

                if (_show)
                {
                    _window = ShowInPopup(position);
                    _window.OnClose += HandleClosed;
                }
                else
                {
                    _window = null;
                }
            }
        }

        private void DrawFunctionButtons(Rect position)
        {
            if (_functionButtonsInfo.IsNullOrEmpty()) return;

            for (int i = _functionButtonsInfo.Count; i-- > 0;)
            {
                var nextInfo = _functionButtonsInfo[i];

                DrawFunctionButton(position, nextInfo);
                position.width += position.height;
            }
        }

        private void DrawFunctionButton(Rect position, FunctionButtonInfo info)
        {
            if (info.action == null) return;

            var size = position.height;

            position.x = position.xMax + 1;
            position.width = size;

            if (GUIBox.ToolbarButton(position, info.icon, GUI.skin.button))
            {
                info.action?.Invoke();
            }
        }

        public override void SetSelection(T selected)
        {
            if (_values.Contains(selected))
            {
                selectedValue = selected;
                base.SetSelection(selected);
            }
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            _tree = tree;
            tree.Config.UseCachedExpandedStates = false;

            tree.Config.DrawSearchToolbar = _enableSearch;
            tree.Selection.SupportsMultiSelect = _enableMultiselect;

            if (_searchFunction != null)
            {
                tree.Config.SearchFunction = _searchFunction;
            }

            for (int i = 0; i < _values.Length; i++)
            {
                var value = _values[i];

                string path = _pathEvaluator != null ?
                    _pathEvaluator.Invoke(value) :
                    value.ToString();

                tree.Add(path, value);
            }

            SetSelection(selectedValue);

            SelectionChanged += col =>
            {
                var value = col.FirstOrDefault();

                if (_values.Contains(value))
                {
                    selectedValue = value;
                    _onSelected?.Invoke(selectedValue);
                    Hide(true);
                }
            };

            SelectionConfirmed += col =>
            {
                var value = col.FirstOrDefault();

                if (_values.Contains(value))
                {
                    selectedValue = value;
                    _onSelected?.Invoke(selectedValue);
                }
            };
        }

        private bool ValidateSelectedValue(T value)
        {
            if (value == null) return true;

            for (int i = 0; i < _values.Length; i++)
            {
                if (value.Equals(_values[i])) return true;
            }

            return false;
        }

        public bool TryGetMenuItemByValue(T value, out OdinMenuItem item)
        {
            item = null;

            if (_tree == null)
                return false;

            if (FindItemByValue(value, _tree.MenuItems, out item))
            {
                return true;
            }

            return false;
        }

        private bool FindItemByValue(T value, List<OdinMenuItem> list, out OdinMenuItem item)
        {
            item = null;

            foreach (var x in list)
            {
                if (x.Value == value)
                {
                    item = x;
                    return true;
                }

                if (FindItemByValue(value, x.ChildMenuItems, out item))
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateItemsNameOnSearch()
        {
            if (_window != null && !string.IsNullOrEmpty(GetSearchTerm()))
            {
                if (_cacheItemNames == null)
                {
                    _cacheItemNames = new Dictionary<T, string>();

                    foreach (var value in _values)
                    {
                        if (TryGetMenuItemByValue(value, out var item))
                        {
                            _cacheItemNames[value] = item.Name;

                            item.Name = item.GetFullPath();
                        }
                    }
                }
            }
            else
            {
                if (_cacheItemNames != null)
                {
                    foreach (var value in _values)
                    {
                        if (_cacheItemNames.TryGetValue(value, out var oldName))
                        {
                            if (TryGetMenuItemByValue(value, out var item))
                            {
                                item.Name = oldName;
                            }
                        }
                    }

                    _cacheItemNames = null;
                }
            }
        }

        public bool DefaultSearchFunction(OdinMenuItem item)
        {
            var value = (T)item.Value;

            if (!_values.Contains(value))
            {
                return false;
            }

            var rawSearchTerm = GetSearchTerm();

            if (string.IsNullOrEmpty(rawSearchTerm))
            {
                return false;
            }

            var searchTerm = rawSearchTerm.ToLower();

            var target = item.GetFullPath().ToLower();

            var contains = target.Contains(searchTerm);

            return contains;
        }

        private void TryClose()
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
        }

        private void HandleClosed()
        {
            if (_window != null)
            {
                _window.OnClose -= HandleClosed;
                _window = null;
            }

            Hide(false);
        }        
    }

    public struct FunctionButtonInfo
    {
        public Action action;
        public EditorIcon icon;
    }
}