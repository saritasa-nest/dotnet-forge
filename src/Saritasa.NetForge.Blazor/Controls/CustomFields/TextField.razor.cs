using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Controls.CustomFields;

/// <summary>
/// Represents text field.
/// </summary>
public partial class TextField : CustomField
{
    /// <summary>
    /// Property value.
    /// </summary>
    public string? PropertyValue
    {
        get => EntityInstance.GetType().GetProperty(Property.Name)?.GetValue(EntityInstance)?.ToString();
        set => EntityInstance.GetType().GetProperty(Property.Name)?.SetValue(EntityInstance, value);
    }

    /// <inheritdoc cref="PropertyMetadataBase.Lines"/>
    public int Lines => Property.IsMultiline ? Property.Lines : 1;

    /// <inheritdoc cref="PropertyMetadataBase.MaxLines"/>
    public int MaxLines => Property.IsMultiline ? Property.MaxLines : 1;

    /// <inheritdoc cref="PropertyMetadataBase.IsAutoGrow"/>
    public bool IsAutoGrow => Property.IsAutoGrow;
}
