using Microsoft.AspNetCore.Components;
using MudBlazor;
using Saritasa.NetForge.Blazor.Domain.UseCases.Interfaces;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.Controls;

/// <summary>
/// Dialog to display a navigation property of the entity.
/// </summary>
public partial class NavigationDialog : ComponentBase
{
    /// <summary>
    /// Entity service.
    /// </summary>
    [Inject]
    public IEntityService EntityService { get; set; } = null!;

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>
    /// Navigation instance. Collection instance in case of navigation collection.
    /// </summary>
    [Parameter]
    public object NavigationInstance { get; set; } = null!;

    /// <summary>
    /// Navigation metadata.
    /// </summary>
    [Parameter]
    public NavigationMetadataDto NavigationMetadata { get; set; } = null!;

    /// <summary>
    /// Properties of the entity.
    /// </summary>
    public IEnumerable<PropertyMetadataDto> Properties { get; set; } = null!;

    /// <summary>
    /// Data grid reference.
    /// </summary>
    public MudDataGrid<object>? DataGrid { get; set; }

    /// <inheritdoc />
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        var entityType = NavigationMetadata.ClrType!.GetGenericArguments().FirstOrDefault()
                         ?? NavigationMetadata.ClrType;

        var entity = await EntityService.GetEntityByTypeAsync(entityType, CancellationToken.None);
        Properties = entity.Properties;
    }

    private void Close()
    {
        MudDialog.Cancel();
    }
}
