using System.Text.RegularExpressions;

namespace IAndIFamilySupport.API.Attributes;

/// <summary>
///     Атрибут для точного совпадения callback data (например, "RESELECT").
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CallbackRouteAttribute(string data) : Attribute
{
    public string Data { get; } = data;
}

/// <summary>
///     Атрибут для сопоставления callback data по регулярному выражению.
///     Например, "^RECORDER_(?.+)$"
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CallbackRoutePatternAttribute(string pattern) : Attribute
{
    public Regex Pattern { get; } = new(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
}