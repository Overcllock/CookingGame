using System;
using System.Linq;
using System.Reflection;

public static class ReflectionExtensions
{
    public static bool HasAttribute<T>(this object obj, bool inherit = false) where T : System.Attribute
    {
        return obj.GetType().HasAttribute<T>(inherit);
    }

    public static bool HasAttribute<T>(this Type type, bool inherit = false) where T : System.Attribute
    {
        return type.GetCustomAttribute<T>(inherit) != null;
    }

    public static T GetCustomAttribute<T>(this Type type, bool inherit = false) where T : System.Attribute
    {
        var attributes = type.GetCustomAttributes(typeof(T), inherit);

        if (attributes == null || attributes.Length == 0) return null;

        T attribute = attributes.First() as T;
        return attribute;
    }

    /// <summary>
    /// Return valid name for type, checking for generic arguments.
    /// </summary>
    public static string GetValidName(this Type type)
    {
        return type.IsGenericType ?
            System.Text.RegularExpressions.Regex.Replace(type.FullName, @"[.\w]+\.(\w+)", "$1") :
            type.HasAlias() ?
            type.GetAlias() :
            type.Name;
    }


    public static Type GetUnderlyingType(this MemberInfo member)
    {
        switch (member.MemberType)
        {
            case MemberTypes.Event:
                return ((EventInfo)member).EventHandlerType;
            case MemberTypes.Field:
                return ((FieldInfo)member).FieldType;
            case MemberTypes.Method:
                return ((MethodInfo)member).ReturnType;
            case MemberTypes.Property:
                return ((PropertyInfo)member).PropertyType;
            default:
                throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
        }
    }

    public static Type GetTypeByName(this Assembly assembly, string typeName)
    {
        var types = assembly.GetTypes();

        for (int i = 0; i < types.Length; i++)
        {
            var nextType = types[i];

            if (nextType.Name == typeName)
                return nextType;
        }

        return null;
    }

    public static string GetTypeName(this object obj)
    {
        return obj.GetType().Name;
    }
    
    public static Type GetDeclaredType<T>(this T obj)
    {
        return obj == null ?
            typeof(T) :
            obj.GetType();
    }
}