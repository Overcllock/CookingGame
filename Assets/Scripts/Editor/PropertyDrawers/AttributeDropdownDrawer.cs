using Game.UI.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public abstract class AttributeDropdownDrawer<T> : AttributePropertyDrawer<T> where T : FilteredPropertyAttribute
{
    public const string NONE = "None";

    protected abstract string controlId { get; }

    protected static string[] _allValues;
    protected GUIPopupSelector<string> _selector;
    protected int _valuesCount;

    protected override void OnInitialize(T attribute, SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            Debug.LogError($"Invalid field type for {controlId}: [ {fieldInfo.FieldType} ]");
            return;
        }

        if (_allValues == null)
        {
            _allValues = GetDropdownValues();
            _valuesCount = _allValues.Length;
        }

        _selector = CreateSelector(attribute, property);
    }

    protected abstract string[] GetDropdownValues();
    protected virtual Action RefreshDelegate() => null;
    protected virtual FunctionButtonInfo[] GetCustomFunctions(SerializedProperty property) => null;
    protected virtual void OnValueChanged(SerializedProperty property)
    {
    }

    protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_valuesCount != _allValues.Length)
        {
            _selector = CreateSelector((T)attribute, property);
            _valuesCount = _allValues.Length;
        }

        if (_selector != null && _selector.selectedValue == null)
        {
            var cantBeEmpty =
                !_allValues.Contains(NONE) &&
                property.stringValue.IsNullOrEmpty();

            if (cantBeEmpty)
            {
                _selector.SetSelection(_allValues.First());
            }
            else
            {
                if (GUI.Button(position, $"Detected invalid value: [ {property.stringValue} ] Press to clear."))
                {
                    _selector.SetSelection(NONE);
                }
            }

            return;
        }

        if (!string.IsNullOrEmpty(label.text))
        {
            GUI.SetNextControlName(controlId);

            var labelPos = position;
            labelPos.width = EditorGUIUtility.labelWidth;
            EditorGUI.SelectableLabel(labelPos, label.text);

            position.xMin += EditorGUIUtility.labelWidth;
        }

        _selector?.Draw(position);

        GUIBox.TryCopyToBuffer(controlId, property);
        if (GUIBox.ShouldPasteFromBuffer(controlId))
        {
            TryPasteValueToProperty(property);
        }
    }

    private void SaveValue(SerializedProperty property, string value)
    {
        if (property.stringValue == value) return;

        if (value == NONE)
        {
            property.stringValue = string.Empty;
            Debug.Log($"[<color=white>{fieldInfo.Name}</color>] {controlId} has been cleared.");
        }
        else
        {
            property.stringValue = value;
            Debug.Log($"[<color=white>{fieldInfo.Name}</color>] {controlId} changed to: [ <color=yellow>{value}</color> ]");
        }

        OnValueChanged(property);
        property.serializedObject.ApplyModifiedProperties();
    }

    private void TryPasteValueToProperty(SerializedProperty property)
    {
        if (_selector.Contains(GUIUtility.systemCopyBuffer))
        {
            GUIBox.PasteFromBuffer(property);
            _selector.SetSelection(property.stringValue);
        }
        else
        {
            Debug.LogWarning(
                $"Cannot paste [ {GUIUtility.systemCopyBuffer} ] from buffer " +
                $"to {attribute.GetType().Name} dropwdown field - value is not supported.");
        }
    }

    private string[] GetValues(string keyword)
    {
        if (keyword.IsNullOrEmpty())
        {
            return _allValues;
        }

        var filtered = new List<string>();

        foreach (var value in _allValues)
        {
            if (CanAddToList(value))
            {
                filtered.Add(value);
            }
        }

        return filtered.ToArray();

        bool CanAddToList(string value)
        {
            if (value == NONE) return true;
            if (value.Contains(keyword, StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }
    }

    private GUIPopupSelector<string> CreateSelector(T attribute, SerializedProperty property)
    {
        var validKeys = GetValues(attribute.keyword);
        var selectedValue = property.stringValue.IsNullOrWhiteSpace() ? NONE : property.stringValue;

        var selector = new GUIPopupSelector<string>(validKeys, selectedValue, (x) => SaveValue(property, x), onRefresh: RefreshDelegate());

        if (fieldInfo.FieldType == typeof(string[]))
        {
            var copyButton = new FunctionButtonInfo
            {
                action = () => GUIBox.CopyToBuffer(property.stringValue),
                icon = EditorIcons.Pen
            };

            var pasteButton = new FunctionButtonInfo
            {
                action = () => TryPasteValueToProperty(property),
                icon = EditorIcons.ArrowDown
            };

            selector.AddFunctionButtons(copyButton, pasteButton);
        }

        selector.AddFunctionButtons(GetCustomFunctions(property));
        return selector;
    }
}