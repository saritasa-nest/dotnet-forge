namespace Saritasa.NetForge.Blazor.MVVM.ViewModels;

/// <summary>
/// Base view model.
/// </summary>
public abstract class BaseViewModel : IDisposable
{
    private bool disposedValue;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Cancellation token.
    /// </summary>
    protected CancellationToken CancellationToken => cancellationTokenSource.Token;

    /// <summary>
    /// Load view model.
    /// </summary>
    public virtual Task LoadAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    /// <summary>
    /// Disposes resources of the view model.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
