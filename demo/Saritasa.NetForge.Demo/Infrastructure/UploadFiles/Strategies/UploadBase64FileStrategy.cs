using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.Blazor.Domain;

namespace Saritasa.NetForge.Demo.Infrastructure.UploadFiles.Strategies;

/// <summary>
/// Strategy when uploaded file converts to base 64 string.
/// </summary>
internal class UploadBase64FileStrategy : IUploadFileStrategy
{
    /// <summary>
    /// Converts given file to base 64 string.
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>File as base 64 string.</returns>
    public async Task<object> UploadFileAsync(IBrowserFile file, CancellationToken cancellationToken)
    {
        var stream = file.OpenReadStream(cancellationToken: cancellationToken);
        var selectedFileBytes = await GetFileBytesAsync(stream, cancellationToken);

        return $"data:{file.ContentType};base64,{Convert.ToBase64String(selectedFileBytes)}";
    }

    private static async Task<byte[]> GetFileBytesAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Returns base 64 <paramref name="fileString"/> as it is.
    /// </summary>
    public string GetFileSource(string fileString)
    {
        return fileString;
    }
}