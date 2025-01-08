using MudBlazor;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.Domain.Enums;
using Saritasa.NetForge.Domain.Exceptions;
using Saritasa.NetForge.UseCases.Common;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Mvvm.ViewModels.EntityDetails;

/// <summary>
/// ViewModel representing details of an entity.
/// </summary>
public class EntityDetailsViewModel : BaseViewModel
{
    /// <summary>
    /// Entity details model.
    /// </summary>
    public EntityDetailsModel Model { get; private set; }

    private readonly IEntityService entityService;
    private readonly AdminOptions adminOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EntityDetailsViewModel(string stringId, IEntityService entityService, AdminOptions adminOptions)
    {
        Model = new EntityDetailsModel { StringId = stringId };

        this.entityService = entityService;
        this.adminOptions = adminOptions;
    }

    /// <summary>
    /// Search string.
    /// </summary>
    public string? SearchString { get; set; }

    /// <summary>
    /// Data grid reference.
    /// </summary>
    public MudDataGrid<object>? DataGrid { get; set; }

    /// <summary>
    /// Whether the entity exists.
    /// </summary>
    public bool IsEntityExists { get; private set; } = true;

    /// <summary>
    /// Whether display search input.
    /// </summary>
    public bool IsDisplaySearchInput { get; set; }

    /// <summary>
    /// Whether instance of the entity can be added.
    /// </summary>
    public bool CanAdd { get; set; }

    /// <summary>
    /// Whether instance of the entity can be edited.
    /// </summary>
    public bool CanEdit { get; set; }

    /// <summary>
    /// Whether instance of the entity can be deleted.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// Whether entity has properties included.
    /// </summary>
    public bool HasProperties => Model.Properties.Any();

    /// <summary>
    /// Selected entities.
    /// </summary>
    public HashSet<object> SelectedEntities { get; set; } = new();

    /// <inheritdoc/>
    public override async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entity = await entityService.GetEntityByIdAsync(Model.StringId, cancellationToken);
            Model = MapModel(entity);

            IsDisplaySearchInput = Model.Properties.Any(property =>
                                   {
                                       if (property is NavigationMetadataDto navigation)
                                       {
                                           return navigation.TargetEntityProperties
                                               .Any(targetProperty => targetProperty.SearchType != SearchType.None);
                                       }

                                       return property.SearchType != SearchType.None;
                                   })
                                   || Model.SearchFunction is not null;

            CanAdd = Model is { CanAdd: true, IsKeyless: false } && HasProperties;
            CanEdit = Model is { CanEdit: true, IsKeyless: false } && HasProperties;
            CanDelete = Model is { CanDelete: true, IsKeyless: false } && HasProperties;
        }
        catch (NotFoundException)
        {
            IsEntityExists = false;
        }
    }

    private EntityDetailsModel MapModel(GetEntityDto entity)
    {
        return Model with
        {
            Id = entity.Id,
            DisplayName = entity.DisplayName,
            PluralName = entity.PluralName,
            Description = entity.Description,
            ClrType = entity.ClrType,
            Properties = entity.Properties,
            SearchFunction = entity.SearchFunction,
            CustomQueryFunction = entity.CustomQueryFunction,
            IsKeyless = entity.IsKeyless,
            CanAdd = entity.CanAdd,
            CanEdit = entity.CanEdit,
            CanDelete = entity.CanDelete
        };
    }

    /// <summary>
    /// Loads entity's data to data grid.
    /// </summary>
    /// <param name="gridState">Information about grid state. For example, current page, page size.</param>
    /// <returns>Grid data collection populated with entity's data.</returns>
    public async Task<GridData<object>> LoadEntityGridDataAsync(GridState<object> gridState)
    {
        var orderBy = gridState.SortDefinitions
            .Select(sort =>
            {
                var column = DataGrid!.RenderedColumns.First(column => column.PropertyName.Equals(sort.SortBy));
                var navigationName = column.UserAttributes["NavigationName"]?.ToString();

                return new OrderByDto
                {
                    FieldName = column.Title,
                    IsDescending = sort.Descending,
                    NavigationName = navigationName
                };
            })
            .ToList();

        if (!orderBy.Any())
        {
            var primaryKeyName = Model.Properties.FirstOrDefault(property => property.IsPrimaryKey)?.Name;

            if (primaryKeyName is not null)
            {
                orderBy.Add(new OrderByDto
                {
                    FieldName = primaryKeyName
                });
            }
        }

        var searchOptions = new SearchOptions
        {
            Page = gridState.Page + 1,
            PageSize = gridState.PageSize,
            SearchString = SearchString,
            OrderBy = orderBy
        };

        var entityData = await entityService
            .SearchDataForEntityAsync(Model.ClrType, Model.Properties, searchOptions, Model.SearchFunction, Model.CustomQueryFunction);

        var data = new GridData<object>
        {
            Items = entityData.Items,
            TotalItems = entityData.Metadata.TotalCount
        };

        return data;
    }

    /// <summary>
    /// Searches data by search string.
    /// </summary>
    public void Search()
    {
        DataGrid?.ReloadServerData();
    }

    /// <summary>
    /// Deletes all selected entities.
    /// </summary>
    public async Task DeleteSelectedEntitiesAsync(CancellationToken cancellationToken)
    {
        if (adminOptions.CallbackOptions.PreDelete is not null)
        {
            await adminOptions.CallbackOptions.PreDelete.Invoke(cancellationToken);
        }

        await entityService.DeleteEntitiesAsync(
            SelectedEntities, SelectedEntities.First().GetType(), cancellationToken);

        DataGrid?.ReloadServerData();
        SelectedEntities.Clear();
    }
}
