using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Domain.Attributes;

/// <summary>
/// Represents an attribute used to provide metadata for the multiline text field.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MultilineTextAttribute : Attribute
{
    /// <inheritdoc cref="PropertyMetadataBase.Lines"/>
    public int Lines { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.MaxLines"/>
    public int MaxLines { get; set; }

    /// <inheritdoc cref="PropertyMetadataBase.IsAutoGrow"/>
    public bool IsAutoGrow { get; set; }
}
