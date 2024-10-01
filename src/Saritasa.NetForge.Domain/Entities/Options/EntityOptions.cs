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
    public string DisplayName { get; set; } = string.Empty;

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
    /// Name of the group which entity belongs to.
    /// </summary>
    public string GroupName { get; set; } = string.Empty;

    /// <summary>
    /// Options for properties of entity.
    /// </summary>
    public ICollection<PropertyOptions> PropertyOptions { get; set; } = new List<PropertyOptions>();

    /// <inheritdoc cref="EntityMetadata.SearchFunction"/>
    public Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? SearchFunction { get; set; }

    /// <summary>
    /// Collection of included navigation names.
    /// </summary>
    public List<string> IncludedNavigations { get; set; } = new();

    /// <summary>
    /// Options for navigations of entity.
    /// </summary>
    public ICollection<NavigationOptions> NavigationOptions { get; set; } = new List<NavigationOptions>();

    /// <inheritdoc cref="EntityMetadata.CustomQueryFunction"/>
    public Func<IServiceProvider?, IQueryable<object>, IQueryable<object>>? CustomQueryFunction { get; set; }

    /// <summary>
    /// Action that called after entity update.
    /// </summary>
    public Action<IServiceProvider?, object, object>? AfterUpdateAction { get; set; }
}
