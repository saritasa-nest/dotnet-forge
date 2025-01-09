namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Enumerable extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Represents non-generic version of <see cref="Enumerable.Cast{T}"/>.
    /// Where <paramref name="targetType"/> used instead of generic type.
    /// </summary>
    /// <param name="source">Source collection.</param>
    /// <param name="targetType">Target type to cast to.</param>
    /// <returns>Cast collection.</returns>
    public static object Cast(this IEnumerable<object> source, Type targetType)
    {
        return typeof(Enumerable)
            .GetMethod(nameof(Enumerable.Cast))!
            .MakeGenericMethod(targetType)
            .Invoke(null, new object[] { source })!;
    }
}
