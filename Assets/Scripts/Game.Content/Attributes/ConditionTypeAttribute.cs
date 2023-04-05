using System.Diagnostics;

[Conditional("UNITY_EDITOR")]
public class ConditionTypeAttribute : FilteredPropertyAttribute
{
    public ConditionTypeAttribute() : base()
    {
    }
    public ConditionTypeAttribute(string keyword) : base(keyword)
    {
    }
}