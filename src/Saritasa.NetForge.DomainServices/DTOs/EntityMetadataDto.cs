﻿namespace Saritasa.NetForge.DomainServices.DTOs;

/// <summary>
/// Represents entity metadata DTO.
/// </summary>
public class EntityMetadataDto
{
    /// <summary>
    /// Name of the entity to display.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Plural name of the entity.
    /// </summary>
    public string? PluralName { get; set; }

    /// <summary>
    /// Entity description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether the entity can be edited.
    /// </summary>
    public bool IsEditable { get; set; }
}
