using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Demo.Infrastructure.UploadFiles.S3Storage;

namespace Saritasa.NetForge.Demo.Infrastructure.UploadFiles.Strategies;

/// <summary>
/// Strategy to upload file to S3.
/// </summary>
internal class UploadFileToS3Strategy : IUploadFileStrategy
{
    private readonly IBlobStorageService blobStorageService;
    private readonly ICloudBlobStorageService cloudBlobStorageService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public UploadFileToS3Strategy(
        IBlobStorageService blobStorageService, ICloudBlobStorageService cloudBlobStorageService)
    {
        this.blobStorageService = blobStorageService;
        this.cloudBlobStorageService = cloudBlobStorageService;
    }

    /// <summary>
    /// Uploads file to S3.
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>File URI.</returns>
    public async Task<object> UploadFileAsync(IBrowserFile file, CancellationToken cancellationToken)
    {
        var fileUri = ContentUriBuilder.Build(file.Name, file.ContentType);
        var fileContent = file.OpenReadStream(cancellationToken: cancellationToken);
        await blobStorageService.UploadAsync(fileUri, fileContent, cancellationToken);

        return fileUri;
    }

    /// <summary>
    /// Gets pre signed URL to the file from S3.
    /// </summary>
    /// <param name="fileString">File content URI.</param>
    /// <returns>Pre signed URL.</returns>
    public string GetFileSource(string fileString)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(10);
        return cloudBlobStorageService.GetPreSignedUrl(fileString, expiresAt);
    }
}
