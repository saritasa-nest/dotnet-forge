using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.MVVM.ViewModels.EntityDetails;

/// <summary>
/// Entity details model.
/// </summary>
public record EntityDetailsModel
{
    /// <inheritdoc cref="EntityMetadata.Id"/>
    public Guid Id { get; set; }

    /// <inheritdoc cref="EntityMetadata.Id"/>
    public string StringId { get; set; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.DisplayName"/>
    public string DisplayName { get; set; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.PluralName"/>
    public string PluralName { get; set; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.Description"/>
    public string Description { get; set; } = string.Empty;

    /// <inheritdoc cref="EntityMetadata.ClrType"/>
    public Type? ClrType { get; set; }

    /// <inheritdoc cref="EntityMetadata.Properties"/>
    public ICollection<PropertyMetadataDto> Properties { get; set; } = new List<PropertyMetadataDto>();

    /// <inheritdoc cref="EntityMetadata.Navigations"/>
    public ICollection<NavigationMetadataDto> Navigations { get; set; } = new List<NavigationMetadataDto>();

    /// <inheritdoc cref="EntityMetadata.SearchFunction"/>
    public Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? SearchFunction { get; set; }

    /// <inheritdoc cref="EntityMetadata.CustomQueryFunction"/>
    public Func<IServiceProvider?, IQueryable<object>, IQueryable<object>>? CustomQueryFunction { get; set; }

    /// <inheritdoc cref="EntityMetadata.IsKeyless"/>
    public bool IsKeyless { get; set; }

    /// <inheritdoc cref="EntityMetadata.CanAdd"/>
    public bool CanAdd { get; set; }

    /// <inheritdoc cref="EntityMetadata.CanEdit"/>
    public bool CanEdit { get; set; }

    /// <inheritdoc cref="EntityMetadata.CanDelete"/>
    public bool CanDelete { get; set; }
}
