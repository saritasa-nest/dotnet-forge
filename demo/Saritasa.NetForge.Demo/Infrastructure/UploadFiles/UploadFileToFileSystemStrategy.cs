﻿using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.Domain;

namespace Saritasa.NetForge.Demo.Infrastructure.UploadFiles;

/// <summary>
/// Strategy to upload file to file system.
/// </summary>
public class UploadFileToFileSystemStrategy : IUploadFileStrategy
{
    /// <summary>
    /// Uploads file to file system.
    /// </summary>
    /// <param name="file">File to upload.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Relative path to uploaded file.</returns>
    public async Task<object> UploadFileAsync(IBrowserFile file, CancellationToken cancellationToken)
    {
        var path = Path.Combine("media", file.Name);
        var pathToCreate = Path.Combine("wwwroot", path);
        Directory.CreateDirectory(Path.GetDirectoryName(pathToCreate)!);

        await using var fileStream = File.Create(pathToCreate);
        await file.OpenReadStream(cancellationToken: cancellationToken).CopyToAsync(fileStream, cancellationToken);

        return path;
    }
}