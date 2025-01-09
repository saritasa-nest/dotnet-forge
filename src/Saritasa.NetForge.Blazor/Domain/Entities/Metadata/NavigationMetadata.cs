namespace Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

/// <summary>
/// Represents metadata about a navigation of an entity model.
/// </summary>
public class NavigationMetadata : PropertyMetadataBase
{
    /// <summary>
    /// Whether this navigation is collection.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Target navigation entity's properties.
    /// </summary>
    public List<PropertyMetadata> TargetEntityProperties { get; set; } = new();

    /// <summary>
    /// Whether this navigation included to an entity.
    /// </summary>
    public bool IsIncluded { get; set; }
}
