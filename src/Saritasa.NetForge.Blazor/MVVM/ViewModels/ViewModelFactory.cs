namespace Saritasa.NetForge.Blazor.MVVM.ViewModels;

/// <summary>
/// View model factory.
/// </summary>
public class ViewModelFactory
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Create a new instance of view model.
    /// </summary>
    /// <typeparam name="TViewModel">Type of the view model.</typeparam>
    /// <param name="viewModelParameters">Any constructor parameters to be passed to the view model.</param>
    /// <returns>Created view model.</returns>
    public TViewModel Create<TViewModel>(params object[] viewModelParameters)
        where TViewModel : BaseViewModel
    {
        return ActivatorUtilities.CreateInstance<TViewModel>(serviceProvider, viewModelParameters);
    }
}
