using System;
using System.Collections.Generic;

public static class StringExtensions
{
    public static string MoveLeft(this string source, int padding)
    {
        return source.PadLeft(source.Length + padding);
    }

    public static string MoveRight(this string source, int padding)
    {
        return source.PadRight(source.Length + padding);
    }
    
    public static string SlashSafe(this string source)
    {
        return source.Replace("\\", "/");
    }

    public static string ToCamelCase(this string source)
    {
        if (!string.IsNullOrEmpty(source) && source.Length > 1)
        {
            return char.ToLowerInvariant(source[0]) + source.Substring(1);
        }

        return source;
    }
    
    public static bool Contains(this string source, string toCheck, StringComparison comparison)
    {
        if (toCheck == null) return false;

        return source?.IndexOf(toCheck, comparison) >= 0;
    }

    public static bool IsNullOrEmpty(this string source)
    {
        return string.IsNullOrEmpty(source);
    }

    public static bool IsNullOrWhiteSpace(this string source)
    {
        return string.IsNullOrWhiteSpace(source);
    }

    public static string FirstCharToUpper(this string s)
    {
        if (string.IsNullOrEmpty(s)) return null;

        char[] chars = s.ToCharArray();
        chars[0] = char.ToUpper(chars[0]);
        return new string(chars);
    }
    
    public static string GetCompositeString<T>(this IEnumerable<T> collection, Func<T, string> getter = null, bool vertical = true, bool numerate = true)
    {
        if (collection == null) return string.Empty;
        return StringUtility.GetCompositeString(collection, vertical, getter, numerate);
    }
}