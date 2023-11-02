﻿using Saritasa.NetForge.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Domain.Entities.Options;

/// <summary>
/// Configure entity in the admin panel.
/// </summary>
public class EntityOptions
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public EntityOptions(Type entityType)
    {
        EntityType = entityType;
    }

    /// <summary>
    /// Type of the Entity to configure.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Entity name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Entity plural name.
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
    /// Options for properties of entity.
    /// </summary>
    public ICollection<PropertyOptions> PropertyOptions { get; set; } = new List<PropertyOptions>();

    /// <inheritdoc cref="EntityMetadata.SearchFunction"/>
    public Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? SearchFunction { get; set; }
}
