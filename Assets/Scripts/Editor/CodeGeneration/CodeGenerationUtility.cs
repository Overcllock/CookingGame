using System;
using UnityEngine;

namespace Infrastructure.CodeGeneration
{
    public static class CodeGenerationUtility
    {
        public static EditorFieldType GetFieldType(Type type)
        {
            if (type == typeof(bool)) return EditorFieldType.Bool;
            if (type == typeof(int)) return EditorFieldType.Int;
            if (type == typeof(float)) return EditorFieldType.Float;
            if (type == typeof(string)) return EditorFieldType.String;
            if (type == typeof(Vector2)) return EditorFieldType.Vector2;
            if (type == typeof(Vector3)) return EditorFieldType.Vector3;

            Debug.LogWarning($"Unsupported type for editor field enum: [{type}]");
            return EditorFieldType.None;
        }

        public static Type GetFieldType(EditorFieldType type)
        {
            switch (type)
            {
                case EditorFieldType.Bool: return typeof(bool);
                case EditorFieldType.Int: return typeof(int);
                case EditorFieldType.Float: return typeof(float);
                case EditorFieldType.String: return typeof(string);
                case EditorFieldType.Vector2: return typeof(Vector2);
                case EditorFieldType.Vector3: return typeof(Vector3);

                default:
                    Debug.LogWarning($"Invalid type of editor field: [{type}]");
                    return null;
            }
        }

        public static string GetClassName(string id, string postfix)
        {
            return (id.Replace(" ", "") + postfix).FirstCharToUpper();
        }

        public static string GetScrobjectConstantsName(Type type)
        {
            return type.Name
                .Replace("Scrobject", "")
                .Replace("Settings", "")
                .Replace("Entry", "")
                .Replace("Content", "")
                .Replace("Config", "")
                .Replace("Type", "")
                .Replace("UI", "")
                + "Type";
        }
    }
}