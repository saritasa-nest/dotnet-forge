using Saritasa.NetForge.Blazor.Domain.UseCases.Interfaces;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.DTOs;

namespace Saritasa.NetForge.Blazor.MVVM.ViewModels;

/// <summary>
/// ViewModel representing entities metadata in the admin panel.
/// </summary>
public class EntitiesViewModel : BaseViewModel
{
    private readonly IEntityService entityService;

    /// <summary>
    /// Collection of entities metadata.
    /// </summary>
    public IEnumerable<EntityMetadataDto> EntitiesMetadata { get; set; } = new List<EntityMetadataDto>();

    /// <summary>
    /// Whether there are any entities to display.
    /// </summary>
    public bool HasEntities => EntitiesMetadata.Any();

    /// <summary>
    /// Constructor.
    /// </summary>
    public EntitiesViewModel(IEntityService entityService)
    {
        this.entityService = entityService;
    }

    /// <summary>
    /// Load entities metadata.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    public override async Task LoadAsync(CancellationToken cancellationToken)
    {
        await GetEntitiesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieve entities metadata from the backend.
    /// </summary>
    private async Task GetEntitiesAsync(CancellationToken cancellationToken)
    {
        var entitiesMetadataDto = await entityService.SearchEntitiesAsync(cancellationToken);
        EntitiesMetadata = entitiesMetadataDto;
    }

    /// <summary>
    /// Retrieve group description based on the first entity in that group.
    /// </summary>
    /// <param name="entities">Entities in the group.</param>
    public string? GetGroupDescription(IEnumerable<EntityMetadataDto> entities)
    {
        return entities.FirstOrDefault()?.Group.Description;
    }
}
