using System.Collections;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Saritasa.NetForge.Blazor.Infrastructure.Helpers;
using Saritasa.NetForge.Blazor.Pages;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.DomainServices.Extensions;
using Saritasa.NetForge.Mvvm.Navigation;
using Saritasa.NetForge.Mvvm.Utils;
using Saritasa.NetForge.Mvvm.ViewModels.EditEntity;
using Saritasa.NetForge.UseCases.Constants;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.Controls;

/// <summary>
/// Columns for entity properties.
/// </summary>
public partial class EntityPropertyColumns : ComponentBase
{
    [Inject]
    private AdminOptions AdminOptions { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private ILogger<EntityPropertyColumns> Logger { get; set; } = default!;

    [Inject]
    private IEntityService EntityService { get; set; } = null!;

    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    /// <summary>
    /// Properties of the entity.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public IEnumerable<PropertyMetadataDto> Properties { get; set; } = null!;

    /// <summary>
    /// Data grid that contains these columns. Used for reloading data grid data.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public MudDataGrid<object>? DataGrid { get; set; }

    /// <summary>
    /// Whether the entity is navigation.
    /// </summary>
    /// <remarks>
    /// For example, <c>Shop</c> has <c>Products</c>. And <c>Products</c> have <c>Supplier</c>.
    /// If the entity is <c>Shop</c> then this method will contain <see langword="false"/>.
    /// Otherwise, <see langword="true"/>.
    /// </remarks>
    [Parameter]
    public bool IsNavigationEntity { get; set; }

    /// <summary>
    /// Whether instance of the entity can be deleted.
    /// </summary>
    [Parameter]
    public bool CanDelete { get; set; }

    /// <summary>
    /// Gets property's display name.
    /// </summary>
    /// <param name="property">Property.</param>
    /// <returns>Display name.</returns>
    private string GetPropertyDisplayName(PropertyMetadataDto property)
    {
        return !string.IsNullOrEmpty(property.DisplayName)
            ? property.DisplayName
            : property.Name;
    }

    /// <summary>
    /// Gets property value via <c>Reflection</c>.
    /// </summary>
    /// <param name="source">Source object.</param>
    /// <param name="property">Property metadata.</param>
    /// <returns>Property value.</returns>
    private object GetPropertyValue(object? source, PropertyMetadataDto property)
    {
        var propertyInfo = source?.GetType().GetProperty(property.Name);
        var value = propertyInfo?.GetValue(source);

        if (value is null || value.ToString() == string.Empty)
        {
            return !string.IsNullOrEmpty(property.EmptyValueDisplay)
                ? property.EmptyValueDisplay
                : DefaultValueConstants.DefaultEmptyPropertyValueDisplay;
        }

        if (property is NavigationMetadataDto navigation)
        {
            value = GetNavigationValue(value, navigation);
        }

        value = FormatValue(value, property.Name);

        if (property.ClrType == typeof(string) && !property.IsImage)
        {
            var stringValue = value.ToString();

            var maxCharacters = property.TruncationMaxCharacters ?? AdminOptions.TruncationMaxCharacters;

            if (maxCharacters != default)
            {
                value = stringValue!.Truncate(maxCharacters);
            }
        }

        return value;
    }

    private static object GetNavigationValue(object navigation, NavigationMetadataDto navigationMetadata)
    {
        var primaryKeys = navigationMetadata.TargetEntityProperties
            .Where(targetProperty => targetProperty.IsPrimaryKey)
            .ToList();

        if (!primaryKeys.Any())
        {
            return navigation;
        }

        if (!navigationMetadata.IsCollection)
        {
            if (primaryKeys.Count == 1)
            {
                return navigation.GetType().GetProperty(primaryKeys[0].Name)!.GetValue(navigation)!;
            }

            return JoinPrimaryKeys(primaryKeys, navigation);
        }

        var primaryKeyValues = new List<object?>();

        foreach (var item in (navigation as IEnumerable)!)
        {
            if (primaryKeys.Count == 1)
            {
                primaryKeyValues.Add(item.GetType().GetProperty(primaryKeys[0].Name)!.GetValue(item));
            }
            else
            {
                primaryKeyValues.Add($"{{ {JoinPrimaryKeys(primaryKeys, item)} }}");
            }
        }

        return $"[ {string.Join(", ", primaryKeyValues)} ]";
    }

    private static string JoinPrimaryKeys(IEnumerable<PropertyMetadataDto> primaryKeys, object navigation)
    {
        var primaryKeyValues = primaryKeys
            .Select(primaryKey => primaryKey.Name)
            .Select(primaryKeyName => navigation.GetType().GetProperty(primaryKeyName)!.GetValue(navigation));

        return string.Join("; ", primaryKeyValues);
    }

    private string FormatValue(object value, string propertyName)
    {
        var propertyMetadata = Properties.FirstOrDefault(property => property.Name == propertyName);
        return DataFormatUtils.GetFormattedValue(value, propertyMetadata?.DisplayFormat,
            propertyMetadata?.FormatProvider);
    }

    private async Task OpenDialogAsync(object navigationInstance, NavigationMetadataDto navigationMetadata)
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraLarge
        };

        var parameters = new DialogParameters
        {
            { nameof(navigationInstance), navigationInstance },
            { nameof(navigationMetadata), navigationMetadata }
        };
        await DialogService.ShowAsync<NavigationDialog>(title: string.Empty, parameters, options);
    }

    /// <summary>
    /// Edit navigation entity by its identifier.
    /// </summary>
    private async Task NavigateToEditing(object navigationInstance, NavigationMetadataDto navigationMetadata)
    {
        var entityMetadata = await EntityService
            .GetEntityByTypeAsync(navigationMetadata.ClrType!, CancellationToken.None);

        var primaryKeyValues = navigationInstance.GetPrimaryKeyValues(navigationMetadata.TargetEntityProperties);
        NavigationService.NavigateTo<EditEntityViewModel>(parameters: [entityMetadata.StringId, primaryKeyValues]);
    }

    /// <summary>
    /// Delete entity.
    /// </summary>
    private async Task DeleteEntityAsync(object entity, CancellationToken cancellationToken)
    {
        try
        {
            if (AdminOptions.CallbackOptions.PreDelete is not null)
            {
                await AdminOptions.CallbackOptions.PreDelete.Invoke(cancellationToken);
            }

            await EntityService.DeleteEntityAsync(entity, entity.GetType(), cancellationToken);

            DataGrid?.ReloadServerData();
        }
        catch (Exception ex)
        {
            var message = ex.InnerException is not null ? ex.InnerException.Message : ex.Message;
            Snackbar.Add($"Failed to delete record due to error: {message}", Severity.Error);

            Logger.LogError(ex, "{Handler}: Failed to delete record {EntityType}", nameof(EntityDetails), entity.GetType());
        }
    }

    private async Task ShowDeleteEntityConfirmationAsync(object source)
    {
        var parameters = new DialogParameters
        {
            { nameof(ConfirmationDialog.ContentText), "Are you sure you want to delete this record?" },
            { nameof(ConfirmationDialog.ButtonText), "Yes" },
            { nameof(ConfirmationDialog.Color), Color.Error }
        };

        var result = await (await DialogService.ShowAsync<ConfirmationDialog>("Delete", parameters)).Result;
        if (!result.Canceled)
        {
            try
            {
                await DeleteEntityAsync(source, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Failed to delete record due to error: {ex.Message}", Severity.Error);
                Logger.LogError("Failed to delete record due to error: {ex.Message}", ex.Message);
            }
        }
    }
}
