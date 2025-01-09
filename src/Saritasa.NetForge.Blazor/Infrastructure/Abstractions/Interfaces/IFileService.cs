namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Service to make operations on files.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Get file content as array of bytes.
    /// </summary>
    /// <param name="stream">Stream.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Content of the file.</returns>
    Task<byte[]> GetFileBytesAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Creates file.
    /// </summary>
    /// <param name="pathToFile">Path to file to create.</param>
    /// <param name="fileContent">Content to write to the new file.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task CreateFileAsync(string pathToFile, byte[] fileContent, CancellationToken cancellationToken);
}
