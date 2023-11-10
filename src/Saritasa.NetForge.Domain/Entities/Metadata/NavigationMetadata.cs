﻿namespace Saritasa.NetForge.Domain.Entities.Metadata;

/// <summary>
/// Represents metadata about a navigation of an entity model.
/// </summary>
public class NavigationMetadata
{
    /// <summary>
    /// The name of the navigation.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this navigation is collection.
    /// </summary>
    public bool IsCollection { get; set; }

    /// <summary>
    /// Target navigation entity's properties.
    /// </summary>
    public List<PropertyMetadata> TargetEntityProperties { get; set; } = new();

}