using Saritasa.NetForge.Domain.Exceptions;
using Microsoft.AspNetCore.Components.Forms;
using Saritasa.NetForge.DomainServices.Extensions;
using Saritasa.NetForge.Mvvm.Navigation;
using Saritasa.NetForge.Mvvm.ViewModels.EntityDetails;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.Mvvm.Utils;

namespace Saritasa.NetForge.Mvvm.ViewModels.CreateEntity;

/// <summary>
/// View model for create entity page.
/// </summary>
public class CreateEntityViewModel : ValidationEntityViewModel
{
    /// <summary>
    /// Entity details model.
    /// </summary>
    public CreateEntityModel Model { get; private set; }

    private readonly ILogger<CreateEntityViewModel> logger;
    private readonly IEntityService entityService;
    private readonly INavigationService navigationService;
    private readonly AdminOptions adminOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    public CreateEntityViewModel(
        string stringId,
        ILogger<CreateEntityViewModel> logger,
        IEntityService entityService,
        INavigationService navigationService,
        AdminOptions adminOptions)
    {
        Model = new CreateEntityModel { StringId = stringId };

        this.logger = logger;
        this.entityService = entityService;
        this.navigationService = navigationService;
        this.adminOptions = adminOptions;
    }

    /// <summary>
    /// Whether the entity exists.
    /// </summary>
    public bool IsEntityExists { get; private set; } = true;

    /// <inheritdoc/>
    public override async Task LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entity = await entityService.GetEntityByIdAsync(Model.StringId, cancellationToken);
            Model = MapModel(entity);
            Model.EntityInstance = Activator.CreateInstance(Model.ClrType!)!;
            Model = Model with
            {
                Properties = Model.Properties
                    .Where(property => property is
                    {
                        IsCalculatedProperty: false,
                        IsValueGeneratedOnAdd: false,
                        IsValueGeneratedOnUpdate: false,
                        IsReadOnly: false
                    })
                    .ToList(),
            };

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

        filesToUpload.Add(property, file);
    }

    private CreateEntityModel MapModel(GetEntityDto entity)
    {
        return Model with
        {
            DisplayName = entity.DisplayName,
            PluralName = entity.PluralName,
            ClrType = entity.ClrType,
            Properties = entity.Properties,
        };
    }

    /// <summary>
    /// Creates entity.
    /// </summary>
    public async Task CreateEntityAsync()
    {
        foreach (var (property, file) in filesToUpload)
        {
            var fileString = await property.UploadFileStrategy!.UploadFileAsync(file, CancellationToken);
            Model.EntityInstance.SetPropertyValue(property.Name, fileString);
        }

        var errors = new List<ValidationResult>();

        // Clear the error on the previous validation.
        FieldErrorModels.ForEach(e => e.ErrorMessage = string.Empty);

        if (!entityService.ValidateEntity(Model.EntityInstance, Model.Properties, ref errors))
        {
            FieldErrorModels.MappingErrorToCorrectField(errors);

            return;
        }

        try
        {
            if (adminOptions.CallbackOptions.PreCreate is not null)
            {
                await adminOptions.CallbackOptions.PreCreate.Invoke(CancellationToken);
            }

            await entityService.CreateEntityAsync(Model.EntityInstance, Model.ClrType!, CancellationToken);
            navigationService.NavigateTo<EntityDetailsViewModel>(parameters: Model.StringId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{handler}: {message}", nameof(CreateEntityViewModel), ex.Message);

            GeneralError = ex.InnerException is not null ? ex.InnerException.Message : ex.Message;
        }
    }
}
