using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.MVVM.Navigation;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.EditEntity;

namespace Saritasa.NetForge.Blazor.Pages;

/// <summary>
/// Edit entity page.
/// </summary>
[Route("/entities/{stringId}/{instancePrimaryKey}")]
public partial class EditEntity : MvvmComponentBase<EditEntityViewModel>
{
    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    [Inject]
    private AdminOptions? AdminOptions { get; set; }

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// Entity id.
    /// </summary>
    [Parameter]
    public string StringId { get; init; } = null!;

    /// <summary>
    /// Entity instance primary key.
    /// </summary>
    [Parameter]
    public string InstancePrimaryKey { get; init; } = null!;

    private readonly List<BreadcrumbItem> breadcrumbItems = new();

    /// <inheritdoc/>
    protected override EditEntityViewModel CreateViewModel()
    {
        return ViewModelFactory.Create<EditEntityViewModel>(StringId, InstancePrimaryKey);
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var adminPanelEndpoint = AdminOptions!.AdminPanelEndpoint;
        var entityDetailsEndpoint = $"{adminPanelEndpoint}/entities/{StringId}";
        var editEntityEndpoint = $"{entityDetailsEndpoint}/{InstancePrimaryKey}";

        breadcrumbItems.Add(new BreadcrumbItem("Entities", adminPanelEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem(ViewModel.Model.PluralName, entityDetailsEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem($"Edit {ViewModel.Model.DisplayName}", editEntityEndpoint));
    }

    private async Task NavigateToPreviousPage()
    {
        await JsRuntime.InvokeVoidAsync("history.back");
    }
}
