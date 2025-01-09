namespace Saritasa.NetForge.Blazor.Domain.Entities.Options;

/// <summary>
/// Group for entities.
/// </summary>
public class EntityGroup
{
    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the group.
    /// </summary>
    public string? Description { get; set; }
}
