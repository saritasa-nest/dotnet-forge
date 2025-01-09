namespace Saritasa.NetForge.Blazor.Domain.Comparers;

/// <summary>
/// Comparer for objects. Uses their <see cref="object.ToString()"/> methods.
/// </summary>
/// <remarks>
/// Useful to compare objects when they have <see cref="object.ToString()"/> overridden.
/// </remarks>
public class ObjectComparer<T> : IEqualityComparer<T>
{
    /// <inheritdoc />
    public bool Equals(T? x, T? y)
    {
        return x?.ToString() == y?.ToString();
    }

    /// <inheritdoc />
    public int GetHashCode(T? obj)
    {
        return obj is null
            ? 0
            : obj.ToString()!.GetHashCode();
    }
}
