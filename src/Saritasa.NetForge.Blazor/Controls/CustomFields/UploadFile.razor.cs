﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.Blazor.Infrastructure.Services;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.Mvvm.ViewModels;

namespace Saritasa.NetForge.Blazor.Controls.CustomFields;

/// <summary>
/// Represents upload file control.
/// </summary>
public partial class UploadFile : CustomField, IRecipient<EntitySubmittedMessage>
{
    [Inject]
    private AdminOptions AdminOptions { get; init; } = null!;

    [Inject]
    private FileService FileService { get; init; } = null!;

    /// <summary>
    /// Property value.
    /// </summary>
    public string? PropertyValue
    {
        get => EntityInstance.GetType().GetProperty(Property.Name)?.GetValue(EntityInstance)?.ToString();
        set => EntityInstance.GetType().GetProperty(Property.Name)?.SetValue(EntityInstance, value);
    }

    private IBrowserFile? selectedFile;

    private byte[]? selectedFileBytes;

    private string? error;

    private async Task UploadFileAsync(IBrowserFile file)
    {
        error = null;

        selectedFile = file;

        try
        {
            // Convert to number of bytes.
            var maxImageSize = 1024 * 1024 * AdminOptions.MaxImageSizeInMb;
            var stream = file.OpenReadStream(maxImageSize);
            selectedFileBytes = await FileService.GetFileBytesAsync(stream);

            PropertyValue = $"data:{selectedFile!.ContentType};base64,{Convert.ToBase64String(selectedFileBytes)}";

            if (Property.IsPathToImage)
            {
                WeakReferenceMessenger.Default.Register(this);
            }
        }
        catch (IOException)
        {
            error = $"Uploaded file exceeds the maximum file size of {AdminOptions.MaxImageSizeInMb} MB.";
        }
    }

    private void RemoveImage()
    {
        PropertyValue = null;
        selectedFile = null;
    }

    /// <summary>
    /// Method to receive entity submit message.
    /// Used to commit an operation to an actual image on the storage.
    /// For example, create image only after submit updating of the entity.
    /// </summary>
    /// <remarks>
    /// For example create entity case: upload file, submit, create entity in database and create file.
    /// </remarks>
    public async void Receive(EntitySubmittedMessage message)
    {
        if (selectedFile is not null)
        {
            var filePath = Path.Combine(AdminOptions.MediaFolder, Property.ImageFolder, selectedFile!.Name);
            var filePathToCreate = Path.Combine(AdminOptions.StaticFilesFolder, filePath);

            try
            {
                await FileService.CreateFileAsync(filePathToCreate, selectedFileBytes!);
                PropertyValue = filePath;
            }
            catch
            {
                error = "Something went wrong with uploading the file.";
                message.HasErrors = true;
                PropertyValue = null;
            }
        }

        WeakReferenceMessenger.Default.Reset();
    }
}