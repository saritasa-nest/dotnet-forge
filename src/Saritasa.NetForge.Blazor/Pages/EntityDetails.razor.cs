using Microsoft.AspNetCore.Components;
using MudBlazor;
using Saritasa.NetForge.Blazor.Controls;
using Saritasa.NetForge.Blazor.Infrastructure.Helpers;
using Saritasa.NetForge.Blazor.MVVM.Navigation;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.CreateEntity;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.EditEntity;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.EntityDetails;

namespace Saritasa.NetForge.Blazor.Pages;

/// <summary>
/// Entity details.
/// </summary>
[Route("/entities/{stringId}")]
public partial class EntityDetails : MvvmComponentBase<EntityDetailsViewModel>
{
    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private ILogger<EntityDetails> Logger { get; set; } = default!;

    /// <summary>
    /// Entity id.
    /// </summary>
    [Parameter]
    public string StringId { get; set; } = null!;

    private readonly List<BreadcrumbItem> breadcrumbItems = new();

    /// <inheritdoc/>
    protected override EntityDetailsViewModel CreateViewModel()
    {
        return ViewModelFactory.Create<EntityDetailsViewModel>(StringId);
    }

    private Dictionary<string, object>? NonKeylessEntityDataGridAttributes { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ViewModel.CanEdit)
        {
            NonKeylessEntityDataGridAttributes = new Dictionary<string, object>
            {
                { "Hover", true },
                { "RowClass", "cursor-pointer" },
                { "RowClick", EventCallback.Factory.Create<DataGridRowClickEventArgs<object>>(this, NavigateToEditing) }
            };
        }

        var adminPanelEndpoint = AdminOptions.AdminPanelEndpoint;

        breadcrumbItems.Add(new BreadcrumbItem("Entities", href: adminPanelEndpoint));
        // Add BreadcrumbItem with the new href value because can not get StringId directly.
        breadcrumbItems.Add(new BreadcrumbItem(ViewModel.Model.PluralName, href: $"{adminPanelEndpoint}/entities/{StringId}"));
    }

    private void NavigateToCreation()
    {
        NavigationService.NavigateTo<CreateEntityViewModel>(parameters: StringId);
    }

    private async Task ShowBulkDeleteEntitiesConfirmationAsync()
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), "Are you sure you want to delete these records?" },
            { nameof(ConfirmationDialog.ButtonText), "Yes" },
            { nameof(ConfirmationDialog.Color), Color.Error }
        };

        var result = await (await DialogService.ShowAsync<ConfirmationDialog>("Bulk Delete", parameters)).Result;
        if (result.Canceled)
        {
            return;
        }

        try
        {
            await ViewModel.DeleteSelectedEntitiesAsync(CancellationToken);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to delete selected records due to error: {ex.Message}", Severity.Error);
            Logger.LogError("Failed to delete selected records due to error: {ex.Message}", ex.Message);
        }
    }

    private void NavigateToEditing(DataGridRowClickEventArgs<object> row)
    {
        var primaryKeyValues = row.Item.GetPrimaryKeyValues(ViewModel.Model.Properties);
        NavigationService.NavigateTo<EditEntityViewModel>(parameters: [StringId, primaryKeyValues]);
    }
}
