using Saritasa.NetForge.Blazor.MVVM.ViewModels;

namespace Saritasa.NetForge.Blazor.MVVM.Navigation;

/// <summary>
/// Service for navigation between pages.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to page with parameters.
    /// </summary>
    /// <typeparam name="TViewModel">View model type.</typeparam>
    /// <param name="forceLoad">If true, bypasses client-side routing and forces the browser to load the new page from the server,
    /// whether or not the URI would normally be handled by the client-side router.</param>
    /// <param name="replace">If true, replaces the current entry in the history stack.
    /// If false, appends the new entry to the history stack.</param>
    /// <param name="parameters">Page route parameters.</param>
    void NavigateTo<TViewModel>(bool forceLoad = false, bool replace = false, params object[] parameters)
        where TViewModel : BaseViewModel;
}
