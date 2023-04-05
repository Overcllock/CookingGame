using UnityEditor;
using UnityEngine;

public abstract class AttributePropertyDrawer<T> : PropertyDrawer where T : PropertyAttribute
{
    private bool _initialized;

    private void Initialize(SerializedProperty property)
    {
        var att = attribute as T;
        OnInitialize(att, property);

        _initialized = true;
    }

    ~AttributePropertyDrawer()
    {
        OnDispose();
    }

    public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!_initialized)
        {
            Initialize(property);
        }

        OnDraw(position, property, label);
    }

    protected virtual void OnInitialize(T attribute, SerializedProperty property)
    {
    }

    protected virtual void OnDispose()
    {
    }

    protected abstract void OnDraw(Rect position, SerializedProperty property, GUIContent label);
}