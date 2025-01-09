namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// <see cref="string"/> extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Truncates <paramref name="value"/>.
    /// </summary>
    /// <param name="value">String to truncate.</param>
    /// <param name="maxCharacters">Maximum characters.</param>
    /// <returns>Truncated string.</returns>
    public static string Truncate(this string value, int maxCharacters)
    {
        return value.Length <= maxCharacters ? value : $"{value[..maxCharacters]}...";
    }
}
