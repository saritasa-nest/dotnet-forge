using Microsoft.AspNetCore.Components;
using Saritasa.NetForge.Blazor.MVVM.ViewModels;

namespace Saritasa.NetForge.Blazor.Pages;

/// <summary>
/// Component base on view model.
/// </summary>
/// <typeparam name="TViewModel">View model type.</typeparam>
public class MvvmComponentBase<TViewModel> : ComponentBase, IDisposable
    where TViewModel : BaseViewModel
{
    private bool disposedValue;
    private CancellationTokenSource? cancellationTokenSource;

    /// <summary>
    /// Cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken => (cancellationTokenSource ??= new CancellationTokenSource()).Token;

    /// <summary>
    /// View model factory.
    /// </summary>
    [Inject]
    protected ViewModelFactory ViewModelFactory { get; private set; } = null!;

    /// <summary>
    /// View model.
    /// </summary>
    public TViewModel ViewModel { get; private set; } = null!;

    /// <inheritdoc />
    protected sealed override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ViewModel = CreateViewModel();

        await OnInitializedComponentAsync(CancellationToken);

        await OnViewModelPreLoadingAsync(CancellationToken);

        await ViewModel.LoadAsync(CancellationToken);
    }

    /// <summary>
    /// Handler for component async initialization.
    /// </summary>
    protected virtual Task OnInitializedComponentAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>
    /// Handler for view model pre loading.
    /// </summary>
    protected virtual Task OnViewModelPreLoadingAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>
    /// Create view model for current component.
    /// </summary>
    /// <returns>Created view model.</returns>
    protected virtual TViewModel CreateViewModel()
        => ViewModelFactory.Create<TViewModel>();

    /// Release some resources.
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
