using Microsoft.AspNetCore.Components;
using MudBlazor;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.MVVM.Navigation;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.CreateEntity;
using Saritasa.NetForge.Blazor.MVVM.ViewModels.EntityDetails;

namespace Saritasa.NetForge.Blazor.Pages;

/// <summary>
/// Create entity page.
/// </summary>
[Route("/entities/{stringId}/create")]
public partial class CreateEntity : MvvmComponentBase<CreateEntityViewModel>
{
    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    [Inject]
    private AdminOptions? AdminOptions { get; set; }

    /// <summary>
    /// Entity id.
    /// </summary>
    [Parameter]
    public string StringId { get; init; } = null!;

    private readonly List<BreadcrumbItem> breadcrumbItems = new();

    /// <inheritdoc/>
    protected override CreateEntityViewModel CreateViewModel()
    {
        return ViewModelFactory.Create<CreateEntityViewModel>(StringId);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var adminPanelEndpoint = AdminOptions!.AdminPanelEndpoint;
        var entityDetailsEndpoint = $"{adminPanelEndpoint}/entities/{StringId}";
        var createEntityEndpoint = $"{entityDetailsEndpoint}/create";

        breadcrumbItems.Add(new BreadcrumbItem("Entities", adminPanelEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem(ViewModel.Model.PluralName, entityDetailsEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem($"Create {ViewModel.Model.DisplayName}", createEntityEndpoint));
    }

    private void NavigateToEntityDetails()
    {
        NavigationService.NavigateTo<EntityDetailsViewModel>(parameters: StringId);
    }
}
