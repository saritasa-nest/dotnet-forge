using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Saritasa.NetForge.Blazor.Domain.Exceptions;
using Saritasa.NetForge.Blazor.Domain.Extensions;
using Saritasa.NetForge.Blazor.Domain.UseCases.Interfaces;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Blazor.MVVM.Utils;

namespace Saritasa.NetForge.Blazor.MVVM.ViewModels.EditEntity;

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

    private readonly ILogger<EditEntityViewModel> logger;
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
        ILogger<EditEntityViewModel> logger,
        IEntityService entityService,
        IOrmDataService dataService,
        IServiceProvider serviceProvider,
        ISnackbar snackbar)
    {
        Model = new EditEntityModel { StringId = stringId };
        InstancePrimaryKey = instancePrimaryKey;

        this.logger = logger;
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
                        IsHidden: false,
                        IsHiddenFromDetails: false
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

        try
        {
            var updatedEntity = await dataService
                .UpdateAsync(Model.EntityInstance!, Model.OriginalEntityInstance!, Model.AfterUpdateAction, CancellationToken);

            // We do clone because UpdateAsync method returns Model.OriginalEntityInstance
            // so we don't want Model.EntityInstance and Model.OriginalEntityInstance to have the same reference.
            Model.EntityInstance = updatedEntity.CloneJson();
            snackbar.Add("Update was completed successfully", Severity.Success);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{handler}: {message}", nameof(EditEntityViewModel), ex.Message);

            GeneralError = ex.InnerException is not null ? ex.InnerException.Message : ex.Message;
        }
    }
}
