using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Domain.Extensions;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

namespace Saritasa.NetForge.Blazor.Controls.CustomFields;

/// <summary>
/// Represents upload image control.
/// </summary>
public partial class UploadImage : CustomField
{
    private bool disposedValue;
    private readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Cancellation token.
    /// </summary>
    private CancellationToken CancellationToken => cancellationTokenSource.Token;

    /// <summary>
    /// Disposes resources of the view model.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

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

    [Inject]
    private AdminOptions AdminOptions { get; init; } = null!;

    [Inject]
    private IFileService FileService { get; init; } = null!;

    [Inject]
    private ILogger<UploadImage> Logger { get; init; } = null!;

    /// <summary>
    /// Method to handle file selection.
    /// </summary>
    [Parameter]
    public Action<PropertyMetadataDto, IBrowserFile?> OnFileSelected { get; set; } = null!;

    /// <summary>
    /// Property value.
    /// </summary>
    private string? PropertyValue => EntityInstance.GetPropertyValue(Property.Name)?.ToString();

    private IBrowserFile? selectedFile;

    private string? error;

    /// <summary>
    /// Used to preview the uploaded image.
    /// </summary>
    private string? imagePreviewSource;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (PropertyValue is not null)
        {
            imagePreviewSource = Property.UploadFileStrategy!.GetFileSource(PropertyValue);
        }
    }

    private async Task UploadImageAsync(IBrowserFile file)
    {
        error = null;

        selectedFile = file;

        try
        {
            // Convert to number of bytes.
            var maxImageSize = 1024 * 1024 * AdminOptions.MaxImageSizeInMb;
            await using var stream = file.OpenReadStream(maxImageSize);
            var selectedFileBytes = await FileService.GetFileBytesAsync(stream, CancellationToken);
            imagePreviewSource =
                $"data:{selectedFile!.ContentType};base64,{Convert.ToBase64String(selectedFileBytes)}";

            OnFileSelected.Invoke(Property, file);
        }
        catch (IOException exception)
        {
            error = $"Uploaded file exceeds the maximum file size of {AdminOptions.MaxImageSizeInMb} MB.";

            Logger.LogInformation(
                exception,
                "Uploaded file exceeds the maximum file size of {MaxImageSize} MB.",
                AdminOptions.MaxImageSizeInMb);
        }
    }

    private void RemoveImage()
    {
        selectedFile = null;
        imagePreviewSource = null;

        OnFileSelected.Invoke(Property, null);
    }
}
