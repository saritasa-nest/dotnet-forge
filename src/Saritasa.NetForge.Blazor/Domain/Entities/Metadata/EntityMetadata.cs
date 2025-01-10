using Saritasa.NetForge.Domain.Entities.Options;

namespace Saritasa.NetForge.Domain.Entities.Metadata;

/// <summary>
/// Metadata of the Database Entity.
/// </summary>
public class EntityMetadata
{
    /// <summary>
    /// The id of the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the entity to display.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Plural name of the entity.
    /// </summary>
    public string PluralName { get; set; } = string.Empty;

    /// <summary>
    /// String entity identifier.
    /// </summary>
    public string StringId { get; set; } = string.Empty;

    /// <summary>
    /// Entity description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Entity CLR type.
    /// </summary>
    public Type? ClrType { get; set; }

    /// <summary>
    /// Whether the entity can be edited.
    /// </summary>
    public bool IsEditable { get; set; } = true;

    /// <summary>
    /// Whether the entity is hidden.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Group which entity belongs to.
    /// </summary>
    public EntityGroup Group { get; set; } = new();

    /// <summary>
    /// A collection of properties metadata associated with this entity.
    /// </summary>
    public List<PropertyMetadata> Properties { get; set; } = new();

    /// <summary>
    /// Represents custom search function.
    /// </summary>
    public Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? SearchFunction { get; set; }

    /// <summary>
    /// A collection of navigations metadata associated with this entity.
    /// </summary>
    public List<NavigationMetadata> Navigations { get; set; } = new();

    /// <summary>
    /// Represents custom query function.
    /// </summary>
    public Func<IServiceProvider?, IQueryable<object>, IQueryable<object>>? CustomQueryFunction { get; set; }

    /// <summary>
    /// Whether the entity has the primary key.
    /// </summary>
    public bool IsKeyless { get; set; }

    /// <summary>
    /// Action that called after entity update.
    /// </summary>
    public Action<IServiceProvider?, object, object>? AfterUpdateAction { get; set; }

    /// <summary>
    /// Determines that entity can be added.
    /// </summary>
    public bool CanAdd { get; set; } = true;

    /// <summary>
    /// Determines that entity can be edited.
    /// </summary>
    public bool CanEdit { get; set; } = true;

    /// <summary>
    /// Determines that entity can be deleted.
    /// </summary>
    public bool CanDelete { get; set; } = true;

    /// <summary>
    /// Custom message when the entity was saved successfully.
    /// </summary>
    public string? EntitySaveMessage { get; set; }
}
