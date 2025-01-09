namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Contains methods that work with reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets value of <paramref name="obj"/> property with given name.
    /// </summary>
    public static object? GetPropertyValue(this object? obj, string propertyName)
    {
        return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    /// <summary>
    /// Sets value of <paramref name="obj"/> property with given name.
    /// </summary>
    public static void SetPropertyValue(this object? obj, string propertyName, object value)
    {
        obj?.GetType().GetProperty(propertyName)?.SetValue(obj, value);
    }
}
