using Microsoft.AspNetCore.Components;
using MudBlazor;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.DTOs;
using Saritasa.NetForge.Blazor.MVVM.ViewModels;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.EntityDetails;
using Saritasa.NetForge.Blazor.Pages;

namespace Saritasa.NetForge.Blazor.Controls;

/// <summary>
/// Entities table component.
/// </summary>
public partial class Entities : MvvmComponentBase<EntitiesViewModel>
{
    [Inject]
    private AdminOptions? AdminOptions { get; set; }

    private void NavigateToDetails(TableRowClickEventArgs<EntityMetadataDto> rowEventArgs)
    {
        NavigationService.NavigateTo<EntityDetailsViewModel>(parameters: rowEventArgs.Item.StringId);
    }

    private TableGroupDefinition<EntityMetadataDto>? groupDefinition;

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        groupDefinition = new()
        {
            Indentation = true,
            Expandable = true,
            Selector = e => e.Group.Name,
            IsInitiallyExpanded = AdminOptions!.GroupHeadersExpanded
        };
    }
}
