using System.Reflection;
using Microsoft.AspNetCore.Components;
using Saritasa.NetForge.Blazor.Infrastructure.Helpers;
using Saritasa.NetForge.Blazor.MVVM.Navigation;
using Saritasa.NetForge.Blazor.MVVM.ViewModels;
using Saritasa.NetForge.Blazor.Pages;

namespace Saritasa.NetForge.Blazor.Infrastructure.Navigation;

/// <summary>
/// Navigation service implementing page navigation.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly NavigationManager navigationManager;
    private readonly IDictionary<Type, Type> viewModelsPages;

    /// <summary>
    /// Constructor.
    /// </summary>
    public NavigationService(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
        viewModelsPages = GetPages();
    }

    /// <inheritdoc />
    public void NavigateTo<TViewModel>(bool forceLoad = false, bool replace = false, params object[] parameters)
        where TViewModel : BaseViewModel
    {
        var viewModelType = typeof(TViewModel);
        NavigateTo(viewModelType, forceLoad, replace, parameters);
    }

    /// <summary>
    /// Navigates to a specific page associated with a ViewModel type.
    /// </summary>
    /// <param name="viewModelType">The type of the ViewModel associated with the target page.</param>
    /// <param name="forceLoad">Whether to force a full page load (true) or use client-side navigation (false).</param>
    /// <param name="replace">Whether to replace the current entry in the browser's navigation history.</param>
    /// <param name="parameters">Optional parameters to include in the route template when navigating.</param>
    /// <exception cref="InvalidOperationException">Thrown when there are no pages for the given ViewModel.</exception>
    private void NavigateTo(Type viewModelType, bool forceLoad = false, bool replace = false,
        params object[] parameters)
    {
        if (!viewModelsPages.TryGetValue(viewModelType, out var pageType))
        {
            throw new InvalidOperationException("There are no registered pages for view model.");
        }

        var routePath = RouteHelper.GetRoute(pageType, parameters);
        navigationManager.NavigateTo(routePath, forceLoad, replace);
    }

    /// <summary>
    /// Retrieve a dictionary mapping ViewModel types to corresponding Page types.
    /// </summary>
    private static Dictionary<Type, Type> GetPages()
    {
        var assembly = Assembly.GetAssembly(typeof(App))!;
        var openGenericType = typeof(MvvmComponentBase<>);

        var viewModelPairs = from type in assembly.GetTypes()
                             where !type.IsAbstract
                                && type.GetCustomAttributes<RouteAttribute>().Any()
                                && type.BaseType is not null
                                && type.BaseType.IsGenericType
                                && type.BaseType.GetGenericTypeDefinition() == openGenericType
                                && type.BaseType.GenericTypeArguments.Length == 1
                             let pageType = type
                             let viewModelType = type.BaseType?.GenericTypeArguments.Single()
                             select KeyValuePair.Create(viewModelType, pageType);

        return new Dictionary<Type, Type>(viewModelPairs);
    }
}
