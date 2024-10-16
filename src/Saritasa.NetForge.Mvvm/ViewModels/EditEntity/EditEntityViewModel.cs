﻿using Saritasa.NetForge.Domain.Exceptions;
using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.DomainServices.Extensions;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;
using System.ComponentModel.DataAnnotations;
using Saritasa.NetForge.Mvvm.Utils;
using System.Reflection;
using MudBlazor;

namespace Saritasa.NetForge.Mvvm.ViewModels.EditEntity;

/// <summary>
/// View model for edit entity page.
/// </summary>
public class EditEntityViewModel : ValidationEntityViewModel
{
    /// <summary>
    /// Entity details model.
    /// </summary>
    public EditEntityModel Model { get; set; }

    /// <summary>
    /// Instance primary key.
    /// </summary>
    public string InstancePrimaryKey { get; set; }

    private readonly IEntityService entityService;
    private readonly IOrmDataService dataService;
    private readonly IServiceProvider serviceProvider;
    private readonly ISnackbar snackbar;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EditEntityViewModel(
        string stringId,
        string instancePrimaryKey,
        IEntityService entityService,
        IOrmDataService dataService,
        IServiceProvider serviceProvider,
        ISnackbar snackbar)
    {
        Model = new EditEntityModel { StringId = stringId };
        InstancePrimaryKey = instancePrimaryKey;

        this.entityService = entityService;
        this.dataService = dataService;
        this.serviceProvider = serviceProvider;
        this.snackbar = snackbar;
    }

    /// <summary>
    /// Whether the entity exists.
    /// </summary>
    public bool IsEntityExists { get; private set; } = true;

    /// <summary>
    /// Errors encountered while entity updating.
    /// </summary>
    public List<ValidationResult>? Errors { get; set; }

    /// <inheritdoc/>
    public override async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entity = await entityService.GetEntityByIdAsync(Model.StringId, cancellationToken);
            Model = MapModel(entity);
            Model = Model with
            {
                Properties = Model.Properties
                    .Where(property => property is
                    {
                        IsCalculatedProperty: false,
                        IsValueGeneratedOnAdd: false,
                        IsValueGeneratedOnUpdate: false,
                        IsHiddenFromListView: false,
                    })
                    .ToList()
            };

            var includedNavigationNames = Model.Properties
                .Where(property => property is NavigationMetadataDto)
                .Select(property => property.Name);

            Model.EntityInstance = await dataService
                .GetInstanceAsync(InstancePrimaryKey, Model.ClrType!, includedNavigationNames, CancellationToken);

            Model.OriginalEntityInstance = Model.EntityInstance.CloneJson();

            FieldErrorModels = Model.Properties
                .Select(property => new FieldErrorModel
                {
                    Property = property
                })
                .ToList();
        }
        catch (NotFoundException)
        {
            IsEntityExists = false;
        }
    }

    private readonly IDictionary<PropertyMetadataDto, IBrowserFile> filesToUpload
        = new Dictionary<PropertyMetadataDto, IBrowserFile>();

    /// <summary>
    /// Handles selected file.
    /// </summary>
    /// <param name="property">File related to this property.</param>
    /// <param name="file">Selected file.</param>
    public void HandleSelectedFile(PropertyMetadataDto property, IBrowserFile? file)
    {
        if (file is null)
        {
            filesToUpload.Remove(property);
            return;
        }

        filesToUpload[property] = file;
    }

    private EditEntityModel MapModel(GetEntityDto entity)
    {
        return Model with
        {
            DisplayName = entity.DisplayName,
            PluralName = entity.PluralName,
            ClrType = entity.ClrType,
            Properties = entity.Properties,
            AfterUpdateAction = entity.AfterUpdateAction
        };
    }

    /// <summary>
    /// Updates entity.
    /// </summary>
    public async Task UpdateEntityAsync()
    {
        foreach (var (property, file) in filesToUpload)
        {
            var fileString = await property.UploadFileStrategy!.UploadFileAsync(file, CancellationToken);
            Model.EntityInstance.SetPropertyValue(property.Name, fileString);
        }

        var errors = new List<ValidationResult>();

        // Clear the error on the previous validation.
        FieldErrorModels.ForEach(e => e.ErrorMessage = string.Empty);

        if (!entityService.ValidateEntity(Model.EntityInstance!, Model.Properties, ref errors))
        {
            FieldErrorModels.MappingErrorToCorrectField(errors);
            Errors = errors;

            return;
        }

        var updatedEntity = await dataService
            .UpdateAsync(Model.EntityInstance!, Model.OriginalEntityInstance!, Model.AfterUpdateAction, CancellationToken);

        await UpdateNavigationProperties();

        // We do clone because UpdateAsync method returns Model.OriginalEntityInstance
        // so we don't want Model.EntityInstance and Model.OriginalEntityInstance to have the same reference.
        Model.EntityInstance = updatedEntity.CloneJson();
        snackbar.Add("Update was completed successfully", Severity.Success);
    }

    private async Task UpdateNavigationProperties()
    {
        foreach (var property in Model.Properties)
        {
            if (property is not NavigationMetadataDto navigation
                || !navigation.EditDetails)
            {
                continue;
            }

            var entityInstance = Model.ClrType.GetProperty(navigation.Name).GetValue(Model.EntityInstance);
            var originalEntityInstance = Model.ClrType.GetProperty(navigation.Name).GetValue(Model.OriginalEntityInstance);
            await dataService.UpdateAsync(entityInstance, originalEntityInstance, null, CancellationToken);
        }
    }
}
