using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.MVVM.ViewModels.CreateEntity;

/// <summary>
/// Model for create entity page.
/// </summary>
public record CreateEntityModel
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
    public object EntityInstance { get; set; } = null!;
}
