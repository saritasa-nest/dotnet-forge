using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.Enums;

namespace Saritasa.NetForge.Blazor.Domain.Entities.Options;

/// <summary>
/// Options for entity property.
/// </summary>
public class PropertyOptions
{
    /// <inheritdoc cref="PropertyMetadataBase.Name"/>
    public string PropertyName { get; set; } = string.Empty;

    /// <inheritdoc cref="PropertyMetadataBase.IsHidden"/>
    public bool IsHidden { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsHiddenFromListView"/>
    public bool IsHiddenFromListView { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsHiddenFromDetails"/>
    public bool IsHiddenFromDetails { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsExcludedFromQuery"/>
    public bool IsExcludedFromQuery { get; set; }

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

    /// <inheritdoc cref="PropertyMetadataBase.SearchType"/>
    public SearchType SearchType { get; set; } = SearchType.None;

    /// <inheritdoc cref="PropertyMetadataBase.IsSortable"/>
    public bool IsSortable { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.EmptyValueDisplay"/>
    public string EmptyValueDisplay { get; set; } = string.Empty;

    /// <inheritdoc cref="PropertyMetadataBase.DisplayAsHtml"/>
    public bool DisplayAsHtml { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsRichTextField"/>
    public bool IsRichTextField { get; set; }

    /// <inheritdoc cref="PropertyMetadata.IsImage"/>
    public bool IsImage { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsReadOnly"/>
    public bool IsReadOnly { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.TruncationMaxCharacters"/>
    public int TruncationMaxCharacters { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsMultiline"/>
    public bool IsMultiline { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.Lines"/>
    public int Lines { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.MaxLines"/>
    public int MaxLines { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsAutoGrow"/>
    public bool IsAutoGrow { get; set; }

    /// <inheritdoc cref="PropertyMetadata.UploadFileStrategy"/>
    public IUploadFileStrategy? UploadFileStrategy { get; set; }

    /// <inheritdoc cref="PropertyMetadata.CanDisplayDetails"/>
    public bool CanDisplayDetails { get; set; }

    /// <inheritdoc cref="PropertyMetadata.CanBeNavigatedToDetails"/>
    public bool CanBeNavigatedToDetails { get; set; }
}
