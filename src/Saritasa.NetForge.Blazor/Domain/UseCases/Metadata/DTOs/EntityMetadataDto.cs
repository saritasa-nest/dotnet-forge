using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;

namespace Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.DTOs;

/// <summary>
/// Represents entity metadata DTO.
/// </summary>
public class EntityMetadataDto
{
    /// <summary>
    /// Name of the entity to display.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Plural name of the entity.
    /// </summary>
    public string PluralName { get; set; } = string.Empty;

    /// <summary>
    /// Entity description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the entity can be edited.
    /// </summary>
    public bool IsEditable { get; set; }

    /// <summary>
    /// Id of the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Group which entity belongs to.
    /// </summary>
    public EntityGroup Group { get; set; } = new();

    /// <inheritdoc cref="EntityMetadata.StringId"/>
    public string StringId { get; set; } = string.Empty;
}
