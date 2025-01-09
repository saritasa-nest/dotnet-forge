namespace Saritasa.NetForge.Blazor.MVVM.Utils;

/// <summary>
/// A utility class for formatting data values based on display format patterns.
/// </summary>
public static class DataFormatUtils
{
    /// <summary>
    /// Formats a data value using a display format and an optional format provider.
    /// </summary>
    /// <param name="value">The data value to format.</param>
    /// <param name="displayFormat">The display format pattern to apply.</param>
    /// <param name="formatProvider">An optional format provider to control the formatting behavior.</param>
    /// <returns>The formatted value as a string.</returns>
    public static string GetFormattedValue(object value, string? displayFormat = null,
        IFormatProvider? formatProvider = null)
    {
        return string.Format(formatProvider, displayFormat ?? "{0}", value);
    }
}
