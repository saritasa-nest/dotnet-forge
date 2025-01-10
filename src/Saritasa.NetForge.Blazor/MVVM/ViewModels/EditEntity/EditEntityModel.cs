using Saritasa.NetForge.Domain.Entities.Metadata;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Mvvm.ViewModels.EditEntity;

/// <summary>
/// Model for edit entity page.
/// </summary>
public record EditEntityModel
{
    /// <inheritdoc cref="EntityMetadata.Id"/>
    public string StringId { get; init; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.DisplayName"/>
    public string DisplayName { get; init; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.PluralName"/>
    public string PluralName { get; init; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.ClrType"/>
    public Type? ClrType { get; init; }

    /// <inheritdoc cref="EntityMetadata.Properties"/>
    public ICollection<PropertyMetadataDto> Properties { get; init; } = new List<PropertyMetadataDto>();

    /// <summary>
    /// Entity instance.
    /// </summary>
    public object? EntityInstance { get; set; }

    /// <summary>
    /// Original unchanged entity instance.
    /// </summary>
    public object? OriginalEntityInstance { get; set; }

    /// <summary>
    /// Action that called after entity update.
    /// </summary>
    public Action<IServiceProvider?, object, object>? AfterUpdateAction { get; init; }

    /// <inheritdoc cref="EntityMetadata.EntitySaveMessage"/>
    public string? EntitySaveMessage { get; init; }
}
