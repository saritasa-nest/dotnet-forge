using Microsoft.AspNetCore.Components.Forms;

namespace Saritasa.NetForge.Blazor.Domain;

/// <summary>
/// Upload file strategy.
/// </summary>
public interface IUploadFileStrategy
{
    /// <summary>
    /// Uploads file.
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    /// Object that represents uploaded file.
    /// For example, when using file system, it can be path to uploaded file.
    /// </returns>
    Task<object> UploadFileAsync(IBrowserFile file, CancellationToken cancellationToken);

    /// <summary>
    /// Gets file source.
    /// </summary>
    /// <param name="fileString">
    /// String that helps with getting file source.
    /// For example: when using S3, it will contain file URI that will be used to generate pre signed URL.
    /// </param>
    /// <returns>File source that gives access to a file.</returns>
    string GetFileSource(string fileString);
}
