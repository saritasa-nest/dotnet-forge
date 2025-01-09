namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Contains methods for interacting with BLOB storage.
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Upload BLOB to the storage.
    /// </summary>
    /// <param name="key">BLOB key.</param>
    /// <param name="stream">Stream with content.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Task.</returns>
    Task UploadAsync(string key, Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Get BLOB from the storage.
    /// </summary>
    /// <param name="key">BLOB key.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Stream with content.</returns>
    Task<Stream> GetAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Get list of BLOB keys from the storage.
    /// </summary>
    /// <param name="pattern">Search pattern.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>List of BLOB keys.</returns>
    Task<IEnumerable<string>> GetListKeysAsync(string pattern, CancellationToken cancellationToken);

    /// <summary>
    /// Remove BLOB from the storage.
    /// </summary>
    /// <param name="key">BLOB key.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the request.</param>
    /// <returns>Task.</returns>
    Task RemoveAsync(string key, CancellationToken cancellationToken);
}
