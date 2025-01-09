using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Saritasa.NetForge.Domain.Entities.Metadata;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;

namespace Saritasa.NetForge.Infrastructure.EfCore.Services;

/// <inheritdoc/>
public class EfCoreMetadataService : IOrmMetadataService
{
    private readonly EfCoreOptions efCoreOptions;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EfCoreMetadataService(EfCoreOptions efCoreOptions, IServiceProvider serviceProvider)
    {
        this.efCoreOptions = efCoreOptions;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IEnumerable<EntityMetadata> GetMetadata()
    {
        var dbContextTypes = efCoreOptions.DbContexts;
        var metadata = new List<EntityMetadata>();

        // Iterate over all DbContexts, collect entities and their properties.
        foreach (var dbContextType in dbContextTypes)
        {
            var dbContextService = serviceProvider.GetService(dbContextType);

            if (dbContextService == null)
            {
                continue;
            }

            var dbContext = (DbContext)dbContextService;

            var model = dbContext.GetService<IDesignTimeModel>().Model;
            var entityTypes = model.GetEntityTypes().ToList();
            metadata.AddRange(entityTypes.Select(GetEntityMetadata));
        }

        return metadata;
    }

    /// <summary>
    /// Retrieve metadata for certain entity type.
    /// </summary>
    /// <param name="entityType">The EF Core entity type to retrieve metadata for.</param>
    /// <returns>An <see cref="EntityMetadata"/> object containing metadata information for the entity type.</returns>
    private static EntityMetadata GetEntityMetadata(IReadOnlyEntityType entityType)
    {
        // GetNavigations retrieves all navigations except many-to-many navigations.
        // GetSkipNavigations retrieves many-to-many navigations
        var navigationsMetadata = entityType
            .GetNavigations()
            .Concat<IReadOnlyNavigationBase>(entityType.GetSkipNavigations())
            .Select(GetNavigationMetadata);

        var entityMetadata = new EntityMetadata
        {
            DisplayName = entityType.ShortName(),
            ClrType = entityType.ClrType,
            Description = entityType.GetComment() ?? string.Empty,
            IsHidden = entityType.IsPropertyBag,
            Properties = GetPropertiesMetadata(entityType),
            Navigations = navigationsMetadata.ToList(),
            IsKeyless = entityType.FindPrimaryKey() is null
        };

        return entityMetadata;
    }

    /// <summary>
    /// Retrieve metadata for a navigation of an entity type.
    /// </summary>
    /// <param name="navigation">The EF Core navigation to retrieve metadata for.</param>
    /// <returns>A <see cref="PropertyMetadata"/> object containing metadata information for the navigation.</returns>
    private static NavigationMetadata GetNavigationMetadata(IReadOnlyNavigationBase navigation)
    {
        var isNullable = false;
        if (navigation is IReadOnlySkipNavigation { ForeignKey: not null } skipNavigation)
        {
            isNullable = !(skipNavigation.ForeignKey.IsRequired || skipNavigation.ForeignKey.IsRequiredDependent);
        }
        else if (navigation is IReadOnlyNavigation readOnlyNavigation)
        {
            isNullable = !readOnlyNavigation.ForeignKey.IsRequiredDependent;
        }

        var navigationMetadata = new NavigationMetadata
        {
            Name = navigation.Name,
            IsCollection = navigation.IsCollection,
            TargetEntityProperties = GetPropertiesMetadata(navigation.TargetEntityType),
            PropertyInformation = navigation.PropertyInfo,
            ClrType = navigation.ClrType,
            IsNullable = isNullable
        };

        return navigationMetadata;
    }

    private static List<PropertyMetadata> GetPropertiesMetadata(IReadOnlyEntityType entityType)
    {
        var propertiesMetadata = entityType.GetProperties().Select(GetPropertyMetadata);

        var reflectionProperties = entityType.ClrType
            .GetProperties()
            .Where(property => !propertiesMetadata.Any(metadata => metadata.Name == property.Name));

        var calculatedProperties = reflectionProperties
            .Where(property => property is { CanRead: true, CanWrite: false })
            .Select(propertyInfo => new PropertyMetadata
            {
                Name = propertyInfo.Name,
                ClrType = propertyInfo.PropertyType,
                IsEditable = false,
                PropertyInformation = propertyInfo,
                IsCalculatedProperty = true
            });
        return propertiesMetadata.Union(calculatedProperties).ToList();
    }

    /// <summary>
    /// Retrieve metadata for a property of an entity type.
    /// </summary>
    /// <param name="property">The EF Core property to retrieve metadata for.</param>
    /// <returns>A <see cref="PropertyMetadata"/> object containing metadata information for the property.</returns>
    private static PropertyMetadata GetPropertyMetadata(IReadOnlyProperty property)
    {
        var propertyMetadata = new PropertyMetadata
        {
            Name = property.Name,
            Description = property.GetComment() ?? string.Empty,
            ClrType = property.ClrType,
            PropertyInformation = property.PropertyInfo,
            IsForeignKey = property.IsForeignKey(),
            IsPrimaryKey = property.IsPrimaryKey(),
            IsNullable = property.IsNullable,
            Order = property.GetColumnOrder(),
            IsShadow = property.IsShadowProperty(),
            IsValueGeneratedOnAdd = property.ValueGenerated.HasFlag(ValueGenerated.OnAdd),
            IsValueGeneratedOnUpdate = property.ValueGenerated.HasFlag(ValueGenerated.OnUpdate),
            IsReadOnly = property.PropertyInfo?.SetMethod is null || property.PropertyInfo.SetMethod.IsPrivate
        };
        return propertyMetadata;
    }
}
