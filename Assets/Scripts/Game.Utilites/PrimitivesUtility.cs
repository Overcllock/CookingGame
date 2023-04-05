using System;
using System.Collections.Generic;

public static class PrimitivesExtensions
{
    public static bool IsNumeric(this object obj)
    {
        return PrimitivesUtility.IsNumeric(obj);
    }

    public static bool IsNumeric(this Type type)
    {
        return PrimitivesUtility.NumericTypes.Contains(type);
    }

    public static string GetAlias(this Type type)
    {
        return PrimitivesUtility.Aliases[type];
    }

    public static bool TryGetAlias(this Type type, out string alias)
    {
        return PrimitivesUtility.Aliases.TryGetValue(type, out alias);
    }

    public static bool HasAlias(this Type type)
    {
        return PrimitivesUtility.Aliases.ContainsKey(type);
    }

    /// <summary>
    /// Checks if type has alias and also checks
    /// for any possible generic arguments.
    /// </summary>
    public static bool ContainsAlias(this Type type)
    {
        if (type.IsGenericType)
        {
            var generics = type.GetGenericArguments();
            for (int i = 0; i < generics.Length; i++)
            {
                if (generics[i].HasAlias()) return true;
            }
        }

        return type.HasAlias();
    }
}

public static class PrimitivesUtility
{
    public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
    {
        typeof(byte),
        typeof(sbyte),
        typeof(int),
        typeof(uint),
        typeof(short),
        typeof(ushort),
        typeof(long),
        typeof(ulong),
        typeof(double),
        typeof(float),
        typeof(decimal)
    };

    public static readonly Dictionary<Type, string> Aliases = new Dictionary<Type, string>()
    {
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(float), "float" },
        { typeof(double), "double" },
        { typeof(decimal), "decimal" },
        { typeof(object), "object" },
        { typeof(bool), "bool" },
        { typeof(char), "char" },
        { typeof(string), "string" },
        { typeof(void), "void" }
    };

    public static bool IsNumeric(object obj)
    {
        return NumericTypes.Contains(obj.GetType());
    }
}