using Game.Utilities;
using System;
using System.Reflection;

namespace Infrastructure.CodeGeneration
{
    public enum EditorFieldType
    {
        None,

        Bool,
        Int,
        Float,
        String,
        Vector2,
        Vector3
    }

    [Serializable]
    public class EditorFieldInfo
    {
        public EditorFieldType Type;
        public string Name;
        public object Value;

        public Type ResultType { get { return CodeGenerationUtility.GetFieldType(Type); } }
        public bool IsValid { get { return Type != EditorFieldType.None && !string.IsNullOrEmpty(Name); } }

        public static EditorFieldInfo Convert(MemberInfo memberInfo)
        {
            return new EditorFieldInfo()
            {
                Name = memberInfo.Name,
                Type = CodeGenerationUtility.GetFieldType(memberInfo.GetUnderlyingType())
            };
        }

        public static EditorFieldInfo Convert(FieldInfo fieldInfo, object value = null)
        {
            return new EditorFieldInfo()
            {
                Name = fieldInfo.Name,
                Type = CodeGenerationUtility.GetFieldType(fieldInfo.FieldType),
                Value = value
            };
        }

        public override string ToString()
        {
            return
                $"Type: [{Type}] " +
                $"Name: [{Name}] " +
                $"Value: [{Value}]";
        }
    }
}