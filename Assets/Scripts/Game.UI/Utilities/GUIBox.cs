using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
using Sirenix.Utilities;
#endif

namespace Game.UI.Utilities
{
    public enum ElementAlignment
    {
        Vertical,
        Horizontal,
        Both
    }

    public static class GUIBox
    {
        public static void AlignedLabel(string label, ElementAlignment alignment, GUIStyle style = null, params GUILayoutOption[] options)
        {
            bool isVertical = alignment == ElementAlignment.Vertical || alignment == ElementAlignment.Both;
            bool isHorizontal = alignment == ElementAlignment.Horizontal || alignment == ElementAlignment.Both;

            if (isVertical)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            if (isHorizontal)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            if (style != null)
            {
                GUILayout.Label(label, style, options);
            }
            else
            {
                GUILayout.Label(label, options);
            }

            if (isHorizontal)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (isVertical)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
        }

        public static bool AlignedButton(string label, ElementAlignment alignment, GUIStyle style = null, params GUILayoutOption[] options)
        {
            bool pressed;

            bool isVertical = alignment == ElementAlignment.Vertical || alignment == ElementAlignment.Both;
            bool isHorizontal = alignment == ElementAlignment.Horizontal || alignment == ElementAlignment.Both;

            if (isVertical)
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }

            if (isHorizontal)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            if (style != null)
            {
                pressed = GUILayout.Button(label, style, options);
            }
            else
            {
                pressed = GUILayout.Button(label, options);
            }

            if (isHorizontal)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (isVertical)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }

            return pressed;
        }

        public static void ShowGrid<T>(List<T> elements, Action<T> drawMethod, int limit,
            ElementAlignment alignment = ElementAlignment.Horizontal, int padding = 0)
        {
            if (alignment == ElementAlignment.Vertical)
            {
                GUILayout.BeginHorizontal();
            }

            int elementsCount = 0;
            bool activeAlignment = false;

            for (int i = 0; i < elements.Count; i++)
            {
                if (limit > 1 && elementsCount == 0)
                {
                    if (i != 0 && padding > 0)
                    {
                        GUILayout.Space(padding);
                    }

                    switch (alignment)
                    {
                        case ElementAlignment.Vertical:
                            GUILayout.BeginVertical();
                            break;
                        case ElementAlignment.Horizontal:
                            GUILayout.BeginHorizontal();
                            break;
                    }

                    activeAlignment = true;
                }

                drawMethod?.Invoke(elements[i]);
                elementsCount++;

                if (limit > 1 && elementsCount == limit)
                {
                    switch (alignment)
                    {
                        case ElementAlignment.Vertical:
                            GUILayout.EndVertical();
                            break;
                        case ElementAlignment.Horizontal:
                            GUILayout.EndHorizontal();
                            break;
                    }

                    activeAlignment = false;
                    elementsCount = 0;
                }
            }

            if (activeAlignment)
            {
                switch (alignment)
                {
                    case ElementAlignment.Vertical:
                        GUILayout.EndVertical();
                        break;
                    case ElementAlignment.Horizontal:
                        GUILayout.EndHorizontal();
                        break;
                }
            }

            if (alignment == ElementAlignment.Vertical)
            {
                GUILayout.EndHorizontal();
            }
        }

        public static int Dropdown(int index, string[] options, ref bool open, ref Vector2 scrollPos, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            if (!open)
            {
                if (GUILayout.Button(options[index], style))
                {
                    open = !open;
                }
            }
            else
            {
                scrollPos = GUILayout.BeginScrollView(scrollPos, layoutOptions);

                for (int i = 0; i < options.Length; i++)
                {
                    if (GUILayout.Button(options[i], style))
                    {
                        index = i;
                        open = false;
                    }
                }

                GUILayout.EndScrollView();
            }

            return index;
        }

        public static void Expansion(float pixels, ElementAlignment alignment = ElementAlignment.Horizontal)
        {
            bool isVertical = alignment == ElementAlignment.Vertical || alignment == ElementAlignment.Both;
            bool isHorizontal = alignment == ElementAlignment.Horizontal || alignment == ElementAlignment.Both;

            if (isVertical)
            {
                GUILayout.BeginVertical();
            }

            if (isHorizontal)
            {
                GUILayout.BeginHorizontal();
            }

            GUILayout.Space(pixels);

            if (isHorizontal)
            {
                GUILayout.EndHorizontal();
            }

            if (isVertical)
            {
                GUILayout.EndVertical();
            }
        }

#if UNITY_EDITOR
        public static string FilterLabel(string filter, string placeholder = "Filter...", GUIStyle style = null, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal();

            var content = EditorGUIUtility.IconContent("d_Search Icon");
            GUILayout.Label(content, GUILayout.Width(18), GUILayout.Height(18));

            var result = style != null ?
                EditorGUILayout.TextField(filter, style, options) :
                EditorGUILayout.TextField(filter, options);

            if (string.IsNullOrWhiteSpace(filter))
            {
                var cachedColor = GUI.color;
                GUI.color = Color.grey;

                if (style != null)
                {
                    EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), " " + placeholder, style);
                }
                else
                {
                    EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), " " + placeholder);
                }

                GUI.color = cachedColor;
            }

            GUILayout.EndHorizontal();

            return result;
        }

        public static void ReadOnlyField(string label, string text)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
                EditorGUILayout.SelectableLabel(text, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
        }

        public static T ObjectField<T>(string label, T obj, bool allowSceneObjects = false) where T : UnityEngine.Object
        {
            return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects);
        }

        public static void ScriptReference(MonoBehaviour instance)
        {
            ScriptReference(instance, instance.GetType());
        }

        public static void ScriptReference(MonoBehaviour instance, Type type)
        {
            AssetReference("Script:", MonoScript.FromMonoBehaviour(instance), type);
        }

        public static void AssetReference(string label, UnityEngine.Object obj)
        {
            AssetReference(label, obj, obj.GetType());
        }

        public static void AssetReference(string label, UnityEngine.Object obj, Type type)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField(label, obj, type, false);
            GUI.enabled = true;
        }

        public static void HorizontalLine(Color color, int thickness = 2, int padding = 10, int offset = 18, int extraWidth = 22)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= offset;
            r.width += extraWidth;
            EditorGUI.DrawRect(r, color);
        }

        public static Vector2 BeginHorizontalScrollView(Vector2 scrollPosition, GUIContent content, GUIStyle style = null)
        {
            var refStyle = style != null ? style : GUI.skin.label;

            var height = refStyle.CalcHeight(content, Screen.width) + EditorGUIUtility.singleLineHeight;
            GUILayoutOption heightOption = GUILayout.Height(height);

            return style != null ?
                GUILayout.BeginScrollView(scrollPosition, style, heightOption) :
                GUILayout.BeginScrollView(scrollPosition, heightOption);
        }

        public static void TryCopyToBuffer(string controlId, SerializedProperty property)
        {
            TryCopyToBuffer(controlId, property.stringValue);
        }

        public static void TryCopyToBuffer(string controlId, string value)
        {
            Event @event = Event.current;

            bool combinationPressed =
                @event.control && @event.keyCode == KeyCode.C &&
                @event.type == EventType.KeyUp;

            if (combinationPressed && GUI.GetNameOfFocusedControl() == controlId)
            {
                CopyToBuffer(value);
            }
        }

        public static void CopyToBuffer(string value)
        {
            GUIUtility.systemCopyBuffer = value;
            Debug.Log($"Copied to buffer: [ <color=white>{GUIUtility.systemCopyBuffer}</color> ]");
        }     
        
        public static bool ShouldPasteFromBuffer(string controlId)
        {
            Event @event = Event.current;

            bool combinationPressed =
                @event.control && @event.keyCode == KeyCode.V &&
                @event.type == EventType.KeyUp;

            return combinationPressed && GUI.GetNameOfFocusedControl() == controlId;
        }

        public static bool TryPasteFromBuffer(string controlId, SerializedProperty property)
        {
            if (ShouldPasteFromBuffer(controlId))
            {
                PasteFromBuffer(property);
                return true;
            }

            return false;
        }

        public static void PasteFromBuffer(SerializedProperty property)
        {
            property.stringValue = GUIUtility.systemCopyBuffer;
            Debug.Log($"Pasted from buffer: [ <color=white>{GUIUtility.systemCopyBuffer}</color> ]");
        }

        public static bool ToolbarButton(Rect rect, EditorIcon icon, GUIStyle style = null, bool ignoreGUIEnabled = false)
        {
            if (style == null)
            {
                style = SirenixGUIStyles.ToolbarButton;
            }

            if (GUI.Button(rect, GUIContent.none, style))
            {
                GUIHelper.RemoveFocusControl();
                GUIHelper.RequestRepaint();
                return true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                --rect.y;
                icon.Draw(rect, 16f);
            }

            if (!ignoreGUIEnabled ||
                Event.current.button != 0 ||
                Event.current.rawType != EventType.MouseDown ||
                !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                return false;
            }

            GUIHelper.RemoveFocusControl();
            GUIHelper.RequestRepaint();
            GUIHelper.PushGUIEnabled(true);
            Event.current.Use();
            GUIHelper.PopGUIEnabled();
            return true;
        }

#endif
    }
}