using System.ComponentModel;
using System.Reflection;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Domain.Enums;

namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Provides extension methods to apply attributes to <see cref="EntityMetadata"/>.
/// </summary>
public static class EntityMetadataAttributesExtensions
{
    /// <summary>
    /// Applies entity-specific attributes to the given <see cref="EntityMetadata"/>.
    /// </summary>
    /// <param name="entityMetadata">The metadata of the entity to which attributes are applied.</param>
    /// <param name="adminOptions">Admin panel options.</param>
    public static void ApplyEntityAttributes(this EntityMetadata entityMetadata, AdminOptions adminOptions)
    {
        foreach (var property in entityMetadata.Properties)
        {
            property.ApplyPropertyAttributes();
        }

        foreach (var navigation in entityMetadata.Navigations)
        {
            navigation.ApplyPropertyAttributes();
        }

        // Try to get the description from the System.ComponentModel.DisplayNameAttribute.
        var displayNameAttribute = entityMetadata.ClrType?.GetCustomAttribute<DisplayNameAttribute>();

        if (!string.IsNullOrEmpty(displayNameAttribute?.DisplayName))
        {
            entityMetadata.DisplayName = displayNameAttribute.DisplayName;
        }

        var descriptionAttribute = entityMetadata.ClrType?.GetCustomAttribute<DescriptionAttribute>();

        if (!string.IsNullOrEmpty(descriptionAttribute?.Description))
        {
            entityMetadata.Description = descriptionAttribute.Description;
        }

        var netForgeEntityAttribute = entityMetadata.ClrType?.GetCustomAttribute<NetForgeEntityAttribute>();

        if (netForgeEntityAttribute == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(netForgeEntityAttribute.Description))
        {
            entityMetadata.Description = netForgeEntityAttribute.Description;
        }

        if (!string.IsNullOrEmpty(netForgeEntityAttribute.DisplayName))
        {
            entityMetadata.DisplayName = netForgeEntityAttribute.DisplayName;
        }

        if (!string.IsNullOrEmpty(netForgeEntityAttribute.PluralName))
        {
            entityMetadata.PluralName = netForgeEntityAttribute.PluralName;
        }

        if (netForgeEntityAttribute.IsHidden)
        {
            entityMetadata.IsHidden = netForgeEntityAttribute.IsHidden;
        }

        entityMetadata.AssignGroupToEntity(netForgeEntityAttribute.GroupName, adminOptions);
    }

    private static void ApplyPropertyAttributes(this PropertyMetadataBase property)
    {
        var descriptionAttribute = property.PropertyInformation?.GetCustomAttribute<DescriptionAttribute>();

        if (!string.IsNullOrEmpty(descriptionAttribute?.Description))
        {
            property.Description = descriptionAttribute.Description;
        }

        var displayNameAttribute = property.PropertyInformation?.GetCustomAttribute<DisplayNameAttribute>();

        if (!string.IsNullOrEmpty(displayNameAttribute?.DisplayName))
        {
            property.DisplayName = displayNameAttribute.DisplayName;
        }

        property.TryApplyMultilineTextAttributeValues();

        var netForgePropertyAttribute = property.PropertyInformation?
            .GetCustomAttribute<NetForgePropertyAttribute>();

        if (netForgePropertyAttribute is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(netForgePropertyAttribute.Description))
        {
            property.Description = netForgePropertyAttribute.Description;
        }

        if (!string.IsNullOrEmpty(netForgePropertyAttribute.DisplayName))
        {
            property.DisplayName = netForgePropertyAttribute.DisplayName;
        }

        if (netForgePropertyAttribute.IsHidden)
        {
            property.IsHidden = netForgePropertyAttribute.IsHidden;
        }

        if (netForgePropertyAttribute.IsHiddenFromListView)
        {
            property.IsHiddenFromListView = netForgePropertyAttribute.IsHiddenFromListView;
        }

        if (netForgePropertyAttribute.IsHiddenFromDetails)
        {
            property.IsHiddenFromDetails = netForgePropertyAttribute.IsHiddenFromDetails;
        }

        if (netForgePropertyAttribute.IsExcludedFromQuery)
        {
            property.IsExcludedFromQuery = netForgePropertyAttribute.IsExcludedFromQuery;
        }

        if (netForgePropertyAttribute.Order >= 0)
        {
            property.Order = netForgePropertyAttribute.Order;
        }

        property.DisplayFormat = netForgePropertyAttribute.DisplayFormat ?? property.DisplayFormat;

        if (netForgePropertyAttribute.SearchType != SearchType.None)
        {
            property.SearchType = netForgePropertyAttribute.SearchType;
        }

        if (netForgePropertyAttribute.IsSortable)
        {
            property.IsSortable = netForgePropertyAttribute.IsSortable;
        }

        if (!string.IsNullOrEmpty(netForgePropertyAttribute.EmptyValueDisplay))
        {
            property.EmptyValueDisplay = netForgePropertyAttribute.EmptyValueDisplay;
        }

        if (netForgePropertyAttribute.DisplayAsHtml)
        {
            property.DisplayAsHtml = netForgePropertyAttribute.DisplayAsHtml;
        }

        if (netForgePropertyAttribute.IsRichTextField)
        {
            property.IsRichTextField = netForgePropertyAttribute.IsRichTextField;
        }

        if (netForgePropertyAttribute.IsReadOnly)
        {
            property.IsReadOnly = netForgePropertyAttribute.IsReadOnly;
        }

        if (netForgePropertyAttribute.TruncationMaxCharacters > 0)
        {
            property.TruncationMaxCharacters = netForgePropertyAttribute.TruncationMaxCharacters;
        }
    }

    private static bool TryApplyMultilineTextAttributeValues(this PropertyMetadataBase property)
    {
        var multilineTextAttribute = property.PropertyInformation?
            .GetCustomAttribute<MultilineTextAttribute>();

        if (multilineTextAttribute is null)
        {
            return false;
        }

        property.IsMultiline = true;

        if (multilineTextAttribute.Lines >= 0)
        {
            property.Lines = multilineTextAttribute.Lines;
        }

        if (multilineTextAttribute.MaxLines >= 0)
        {
            property.MaxLines = multilineTextAttribute.MaxLines;
        }

        if (multilineTextAttribute.IsAutoGrow)
        {
            property.IsAutoGrow = multilineTextAttribute.IsAutoGrow;
        }

        return true;
    }
}
