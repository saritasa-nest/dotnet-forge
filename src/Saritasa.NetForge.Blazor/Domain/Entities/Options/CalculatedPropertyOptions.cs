using Saritasa.NetForge.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Domain.Entities.Options;

/// <summary>
/// Options for entity calculated property.
/// </summary>
public class CalculatedPropertyOptions
{
    /// <inheritdoc cref="PropertyMetadataBase.Name"/>
    public string PropertyName { get; set; } = string.Empty;

    /// <inheritdoc cref="PropertyMetadataBase.IsHiddenFromListView"/>
    public bool IsHiddenFromListView { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.DisplayName"/>
    public string? DisplayName { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.Description"/>
    public string? Description { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.Order"/>
    public int? Order { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.Order"/>
    public string? DisplayFormat { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.FormatProvider"/>
    public IFormatProvider? FormatProvider { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.EmptyValueDisplay"/>
    public string EmptyValueDisplay { get; set; } = string.Empty;

    /// <inheritdoc cref="PropertyMetadataBase.DisplayAsHtml"/>
    public bool DisplayAsHtml { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.TruncationMaxCharacters"/>
    public int TruncationMaxCharacters { get; set; }
}
