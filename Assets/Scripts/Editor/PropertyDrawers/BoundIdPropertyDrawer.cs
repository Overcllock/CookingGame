using Game.Content;
using Game.UI.Utilities;
using Game.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(BoundIdAttribute))]
public class BoundIdPropertyDrawer : AttributePropertyDrawer<BoundIdAttribute>
{
    private IDrawer _drawer;

    protected override void OnInitialize(BoundIdAttribute attribute, SerializedProperty property)
    {
        var assetType = GetAssetType(attribute);

        if (assetType != null && assetType.GetInterface(nameof(IIdentifiable)) == null)
        {
            Debug.LogError(
                $"Using invalid type for Bound Id attribute: [ {assetType.Name} ] " +
                $"Type has to be {nameof(IIdentifiable)}");
        }

        if (fieldInfo.FieldType != typeof(string) && fieldInfo.FieldType != typeof(string[]))
        {
            Debug.LogError(
                $"Trying to use {nameof(BoundIdAttribute)} for non-string property!",
                property.serializedObject.targetObject);
        }

        _drawer = GetDrawer(fieldInfo.FieldType, assetType, property);
    }

    protected override void OnDispose()
    {
        _drawer?.Dispose();
    }

    protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
    {
        _drawer?.Draw(position, property, label);
    }

    private IDrawer GetDrawer(Type fieldType, Type assetType, SerializedProperty property)
    {
        switch (fieldType)
        {
            case Type _ when fieldType == typeof(string):
                return new SingleIdDrawer(assetType, property);

            case Type _ when fieldType == typeof(string[]):
                return new SingleIdDrawer(assetType, property);

            default:

                Debug.LogError(
                    $"Trying to use {nameof(BoundIdAttribute)} for non-string property!",
                    property.serializedObject.targetObject);

                return null;
        }
    }

    private Type GetAssetType(BoundIdAttribute attribute)
    {
        if (attribute.type != null)
        {
            if (attribute.type.GetInterface(nameof(IIdentifiable)) != null)
            {
                return attribute.type;
            }
            else
            {
                Debug.LogError(
                    $"Using invalid type for Bound Id attribute: [ {attribute.type.Name} ] " +
                    $"Type has to be {nameof(IIdentifiable)}");
            }
        }

        if (!attribute.typeName.IsNullOrEmpty())
        {
            var assemblies = ReflectionUtility.GetAssemblies("Game", "Game.Content");

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetTypeByName(attribute.typeName);
                if (type != null) return type;
            }

            Debug.LogError($"Invalid type name for Bound Id attribute: [ {attribute.typeName} ]");
        }

        Debug.LogError($"Could not find valid type for attribute: [ {attribute} ]");
        return null;
    }

    private interface IDrawer : IDisposable
    {
        void Draw(Rect position, SerializedProperty property, GUIContent label);
    }

    private class SingleIdDrawer : IDrawer
    {
        private const string CONTROL_ID = "BoundIdField";

        private Type _type;
        private WeakReference<IIdentifiable> _reference;

        private AssetInlineDrawer _detailedInfo;
        private bool _showDetailedInfo;

        public SingleIdDrawer(Type type, SerializedProperty property)
        {
            _type = type;
            _reference = new WeakReference<IIdentifiable>(FindSelectedAsset(_type, property.stringValue));

            UpdateInlineDrawer();
        }

        public void Dispose()
        {
            EditorAssetsCache.ResetCache();
        }

        private IIdentifiable FindSelectedAsset(Type type, string id)
        {
            if (id.IsNullOrEmpty()) return null;

            var assets = EditorAssetsCache.GetCachedAssets(type);

            foreach (var nextAsset in assets)
            {
                if (nextAsset is IIdentifiable identifiable &&
                    identifiable.id == id)
                {
                    return identifiable;
                }
            }

            return null;
        }

        public void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            if (_reference.TryGetTarget(out IIdentifiable identifiable))
            {
                var pos = new Rect(position.x, position.y, position.width / 2.5f, position.height);
                _showDetailedInfo = SirenixEditorGUI.Foldout(pos, _showDetailedInfo, new GUIContent(""));
            }

            GUI.SetNextControlName(CONTROL_ID);
            identifiable = (IIdentifiable)EditorGUI.ObjectField(position, label.text, (Object)identifiable, _type, false);

            if (SirenixEditorGUI.BeginFadeGroup(this, _showDetailedInfo))
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                _detailedInfo.Draw();
                GUILayout.EndVertical();
            }

            SirenixEditorGUI.EndFadeGroup();

            if (EditorGUI.EndChangeCheck())
            {
                if (identifiable != null && identifiable.id.IsNullOrEmpty())
                {
                    Debug.LogError($"Failed to assign identifiable - id is empty.");
                }
                else
                {
                    _reference.SetTarget(identifiable);

                    property.stringValue = identifiable?.id;
                    UpdateInlineDrawer();
                }
            }

            EditorGUI.EndProperty();

            if (Event.current.type == EventType.Repaint)
            {
                if (_reference.TryGetTarget(out var _) &&
                    property.stringValue.IsNullOrEmpty())
                {
                    _reference.SetTarget(null);
                    UpdateInlineDrawer();
                }
            }

            GUIBox.TryCopyToBuffer(CONTROL_ID, property);
            if (GUIBox.TryPasteFromBuffer(CONTROL_ID, property))
            {
                _reference.SetTarget(FindSelectedAsset(_type, property.stringValue));
                UpdateInlineDrawer();
            }
        }

        private void UpdateInlineDrawer()
        {
            if (_detailedInfo == null)
            {
                _detailedInfo = new AssetInlineDrawer(false);
            }

            if (_reference.TryGetTarget(out var identifiable))
            {
                _detailedInfo.SetAsset((Object)identifiable);
            }
            else
            {
                _detailedInfo.DestroyEditor();
            }
        }
    }
    private class IdsArrayDrawer : IDrawer
    {
        private class IdsArrayElementInfo
        {
            public IIdentifiable asset;
            public bool showDetailedInfo;
            public string id { get { return asset?.id; } }

            public IdsArrayElementInfo()
            {
            }
            public IdsArrayElementInfo(IIdentifiable identifiable)
            {
                this.asset = identifiable;
            }
        }

        private Type _assetType;

        private ReorderableList _idsList;
        private List<IdsArrayElementInfo> _elements;

        public IdsArrayDrawer(Type type, SerializedProperty property)
        {
            _assetType = type;
            _idsList = new ReorderableList(property.serializedObject, property, true, true, true, true);

            RepopulateElements();

            _idsList.drawElementCallback += DrawLinkedElement;
            _idsList.onAddCallback += AddItem;
            _idsList.onRemoveCallback += RemoveItem;
            _idsList.onReorderCallback += Reorder;
        }

        private void RepopulateElements()
        {
            if (_assetType == null) return;
            if (_idsList == null || _idsList.serializedProperty.arraySize <= 0) return;

            var allAssets = AssetDatabaseUtility.GetAssets(_assetType);
            _elements = new List<IdsArrayElementInfo>();

            for (int i = 0; i < _idsList.serializedProperty.arraySize; i++)
            {
                string id = _idsList.serializedProperty.GetArrayElementAtIndex(i).stringValue;
                var asset = FindSelectedAsset(allAssets, id);

                _elements.Add(new IdsArrayElementInfo(asset));
            }
        }

        private IIdentifiable FindSelectedAsset(Object[] allAssets, string id)
        {
            if (id.IsNullOrEmpty()) return null;

            foreach (var nextAsset in allAssets)
            {
                if (nextAsset is IIdentifiable identifiable &&
                    identifiable.id == id)
                {
                    return identifiable;
                }
            }

            return null;
        }

        public void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_assetType == null || _idsList == null) return;

            property.serializedObject.Update();

            _idsList.DoLayoutList();

            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawLinkedElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var elementInfo = _elements[index];

            EditorGUI.BeginChangeCheck();

            elementInfo.showDetailedInfo = EditorGUI.Foldout(rect, elementInfo.showDetailedInfo, "");
            elementInfo.asset = (IIdentifiable)EditorGUI.ObjectField(rect, (Object)elementInfo.asset, _assetType, false);

            if (EditorGUI.EndChangeCheck())
            {
                var element = _idsList.serializedProperty.GetArrayElementAtIndex(index);
                element.stringValue = elementInfo.id;
            }
        }

        private void AddItem(ReorderableList list)
        {
            list.serializedProperty.arraySize++;
            _elements.Add(new IdsArrayElementInfo());
        }

        private void RemoveItem(ReorderableList list)
        {
            list.serializedProperty.arraySize--;
            _elements.RemoveAt(list.index);
        }

        private void Reorder(ReorderableList list)
        {
            RepopulateElements();
        }

        public void Dispose()
        {
        }
    }
}