namespace Saritasa.NetForge.Blazor.Domain.Attributes;

/// <summary>
/// Represents an attribute used to provide metadata for the entity.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NetForgeEntityAttribute : Attribute
{
    /// <summary>
    /// Name of the entity to display.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Plural name of the entity.
    /// </summary>
    public string PluralName { get; set; } = string.Empty;

    /// <summary>
    /// Entity description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Whether the entity is hidden from the view.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Name of the group which entity belongs to.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;
}
