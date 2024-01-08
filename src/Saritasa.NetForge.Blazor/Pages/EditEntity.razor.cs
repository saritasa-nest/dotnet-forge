﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MudBlazor;
using Saritasa.NetForge.Blazor.Infrastructure;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.Mvvm.Navigation;
using Saritasa.NetForge.Mvvm.ViewModels.EditEntity;
using Saritasa.NetForge.Mvvm.ViewModels.EntityDetails;

namespace Saritasa.NetForge.Blazor.Pages;

/// <summary>
/// Edit entity page.
/// </summary>
[Route("/entities/{stringId}/edit")]
public partial class EditEntity : MvvmComponentBase<EditEntityViewModel>
{
    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    [Inject]
    private AdminOptions? AdminOptions { get; set; }

    [Inject]
    private StateContainer StateContainer { get; set; } = null!;

    /// <summary>
    /// Entity id.
    /// </summary>
    [Parameter]
    public string StringId { get; init; } = null!;

    private readonly List<BreadcrumbItem> breadcrumbItems = new();

    /// <inheritdoc/>
    protected override EditEntityViewModel CreateViewModel()
    {
        return ViewModelFactory.Create<EditEntityViewModel>(StringId);
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ViewModel.EntityModel = StateContainer.Value;

        var adminPanelEndpoint = AdminOptions!.AdminPanelEndpoint;
        var entityDetailsEndpoint = $"{adminPanelEndpoint}/entities/{StringId}";
        var editEntityEndpoint = $"{entityDetailsEndpoint}/edit";

        breadcrumbItems.Add(new BreadcrumbItem("Entities", adminPanelEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem(ViewModel.Model.PluralName, entityDetailsEndpoint));
        breadcrumbItems.Add(new BreadcrumbItem($"Edit {ViewModel.Model.DisplayName}", editEntityEndpoint));
    }

    private void NavigateToEntityDetails()
    {
        NavigationService.NavigateTo<EntityDetailsViewModel>(parameters: StringId);
    }
}