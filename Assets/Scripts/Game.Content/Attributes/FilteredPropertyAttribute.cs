using System.Diagnostics;
using UnityEngine;

[Conditional("UNITY_EDITOR")]
public abstract class FilteredPropertyAttribute : PropertyAttribute
{
    public string keyword { get; }
    public FilteredPropertyAttribute() { }

    /// <summary>
    /// Filters all initial keys.
    /// Only intersections will be displayed.
    /// </summary>
    public FilteredPropertyAttribute(string keyword)
    {
        this.keyword = keyword;
    }
}