﻿using System.Reflection;

namespace Saritasa.NetForge.Domain.Entities.Metadata;

/// <summary>
/// Represents metadata about a property of an entity model.
/// </summary>
public class PropertyMetadata
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the property. If not empty this name will be displayed instead of <see cref="Name"/>.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The description of the property.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The <see cref="PropertyInfo"/> representing the property in .NET reflection.
    /// </summary>
    public PropertyInfo? PropertyInformation { get; set; }

    /// <summary>
    /// Whether the property is nullable.
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Whether the property is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Whether the property is a foreign key.
    /// </summary>
    public bool IsForeignKey { get; set; }

    /// <summary>
    /// Whether the property is editable.
    /// </summary>
    public bool IsEditable { get; set; } = true;

    /// <summary>
    /// Whether the property is searchable.
    /// </summary>
    public bool IsSearchable { get; set; }

    /// <summary>
    /// The order of the property.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// The type of the property.
    /// </summary>
    public Type? ClrType { get; set; }

    /// <summary>
    /// Whether the property is hidden from the view.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets a value indicating whether this is a shadow property (does not have a
    /// corresponding property in the entity class).
    /// </summary>
    public bool IsShadow { get; set; }

    /// <summary>
    /// Whether the property value is generated on entity database create.
    /// </summary>
    public bool IsValueGeneratedOnAdd { get; set; }

    /// <summary>
    /// Whether the property value is generated on entity database update.
    /// </summary>
    public bool IsValueGeneratedOnUpdate { get; set; }

    /// <summary>
    /// Define position in column order.
    /// </summary>
    public short? Position { get; set; }
}
