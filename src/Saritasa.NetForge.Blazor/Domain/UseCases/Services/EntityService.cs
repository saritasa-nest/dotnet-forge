using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Saritasa.NetForge.Domain.Dtos;
using Saritasa.NetForge.Domain.Entities.Metadata;
using Saritasa.NetForge.Domain.Enums;
using Saritasa.NetForge.Domain.Exceptions;
using Saritasa.NetForge.DomainServices.Extensions;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.UseCases.Common;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.DTOs;
using Saritasa.NetForge.UseCases.Metadata.GetEntityById;
using Saritasa.NetForge.UseCases.Metadata.Services;
using Saritasa.Tools.Common.Pagination;
using Saritasa.Tools.Common.Utils;

namespace Saritasa.NetForge.UseCases.Services;

/// <inheritdoc />
public class EntityService : IEntityService
{
    private readonly AdminMetadataService adminMetadataService;
    private readonly IOrmDataService dataService;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Constructor for <see cref="EntityService"/>.
    /// </summary>
    public EntityService(
        AdminMetadataService adminMetadataService, IOrmDataService dataService, IServiceProvider serviceProvider)
    {
        this.adminMetadataService = adminMetadataService;
        this.dataService = dataService;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public Task<IEnumerable<EntityMetadataDto>> SearchEntitiesAsync(CancellationToken cancellationToken)
    {
        var metadata = adminMetadataService.GetMetadata()
            .Where(entityMetadata => !entityMetadata.IsHidden);
        return Task.FromResult(MapEntityMetadata(metadata));
    }

    private static IEnumerable<EntityMetadataDto> MapEntityMetadata(IEnumerable<EntityMetadata> metadata)
    {
        return metadata.Select(entityMetadata => new EntityMetadataDto
        {
            DisplayName = entityMetadata.DisplayName,
            PluralName = entityMetadata.PluralName,
            Description = entityMetadata.Description,
            IsEditable = entityMetadata.IsEditable,
            Id = entityMetadata.Id,
            Group = entityMetadata.Group,
            StringId = entityMetadata.StringId
        });
    }

    /// <inheritdoc />
    public Task<GetEntityDto> GetEntityByIdAsync(string stringId, CancellationToken cancellationToken)
    {
        var metadata = adminMetadataService
            .GetMetadata()
            .FirstOrDefault(entityMetadata => entityMetadata.StringId.Equals(stringId, StringComparison.OrdinalIgnoreCase));

        if (metadata is null)
        {
            throw new NotFoundException("Metadata for entity was not found.");
        }

        var metadataDto = GetEntityMetadataDto(metadata);

        return Task.FromResult(metadataDto);
    }

    /// <inheritdoc />
    public Task<GetEntityDto> GetEntityByTypeAsync(Type entityType, CancellationToken cancellationToken)
    {
        var metadata = adminMetadataService
            .GetMetadata()
            .FirstOrDefault(entityMetadata => entityMetadata.ClrType == entityType);

        if (metadata is null)
        {
            throw new NotFoundException("Metadata for entity was not found.");
        }

        var metadataDto = GetEntityMetadataDto(metadata);

        return Task.FromResult(metadataDto);
    }

    private static GetEntityDto GetEntityMetadataDto(EntityMetadata metadata)
    {
        var displayableProperties = metadata.Properties
            .Where(property => property is { IsForeignKey: false });

        var propertyDtos = displayableProperties.Select(MapProperty);

        var displayableNavigations = metadata.Navigations
            .Where(navigation => navigation is { IsIncluded: true });

        var navigationDtos = displayableNavigations.Select(MapNavigation);

        propertyDtos = propertyDtos.Union(navigationDtos);

        return MapGetEntityDto(metadata) with { Properties = propertyDtos.ToList() };
    }

    private static PropertyMetadataDto MapProperty(PropertyMetadata property)
    {
        return new PropertyMetadataDto
        {
            Name = property.Name,
            DisplayName = property.DisplayName,
            Description = property.Description,
            IsPrimaryKey = property.IsPrimaryKey,
            ClrType = property.ClrType,
            SearchType = property.SearchType,
            Order = property.Order,
            DisplayFormat = property.DisplayFormat,
            FormatProvider = property.FormatProvider,
            IsCalculatedProperty = property.IsCalculatedProperty,
            IsSortable = property.IsSortable,
            EmptyValueDisplay = property.EmptyValueDisplay,
            IsHidden = property.IsHidden,
            IsHiddenFromListView = property.IsHiddenFromListView,
            IsHiddenFromDetails = property.IsHiddenFromDetails,
            IsExcludedFromQuery = property.IsExcludedFromQuery,
            DisplayAsHtml = property.DisplayAsHtml,
            IsValueGeneratedOnAdd = property.IsValueGeneratedOnAdd,
            IsValueGeneratedOnUpdate = property.IsValueGeneratedOnUpdate,
            IsRichTextField = property.IsRichTextField,
            IsImage = property.IsImage,
            UploadFileStrategy = property.UploadFileStrategy,
            IsReadOnly = property.IsReadOnly,
            TruncationMaxCharacters = property.TruncationMaxCharacters,
            IsNullable = property.IsNullable,
            IsMultiline = property.IsMultiline,
            Lines = property.Lines,
            MaxLines = property.MaxLines,
            IsAutoGrow = property.IsAutoGrow,
            CanDisplayDetails = property.CanDisplayDetails,
            CanBeNavigatedToDetails = property.CanBeNavigatedToDetails
        };
    }

    private static NavigationMetadataDto MapNavigation(NavigationMetadata navigation)
    {
        return new NavigationMetadataDto
        {
            IsCollection = navigation.IsCollection,
            TargetEntityProperties = navigation.TargetEntityProperties.ConvertAll(MapProperty),
            Name = navigation.Name,
            DisplayName = navigation.DisplayName,
            Description = navigation.Description,
            ClrType = navigation.ClrType,
            SearchType = navigation.SearchType,
            Order = navigation.Order,
            DisplayFormat = navigation.DisplayFormat,
            FormatProvider = navigation.FormatProvider,
            IsSortable = navigation.IsSortable,
            EmptyValueDisplay = navigation.EmptyValueDisplay,
            IsHidden = navigation.IsHidden,
            IsHiddenFromListView = navigation.IsHiddenFromListView,
            IsHiddenFromDetails = navigation.IsHiddenFromDetails,
            IsExcludedFromQuery = navigation.IsExcludedFromQuery,
            DisplayAsHtml = navigation.DisplayAsHtml,
            IsRichTextField = navigation.IsRichTextField,
            IsReadOnly = navigation.IsReadOnly,
            TruncationMaxCharacters = navigation.TruncationMaxCharacters,
            IsNullable = navigation.IsNullable,
            IsMultiline = navigation.IsMultiline,
            Lines = navigation.Lines,
            MaxLines = navigation.MaxLines,
            IsAutoGrow = navigation.IsAutoGrow
        };
    }

    private static GetEntityDto MapGetEntityDto(EntityMetadata entity)
    {
        return new GetEntityDto
        {
            Id = entity.Id,
            DisplayName = entity.DisplayName,
            PluralName = entity.PluralName,
            StringId = entity.StringId,
            Description = entity.Description,
            ClrType = entity.ClrType,
            SearchFunction = entity.SearchFunction,
            CustomQueryFunction = entity.CustomQueryFunction,
            IsKeyless = entity.IsKeyless,
            AfterUpdateAction = entity.AfterUpdateAction,
            CanAdd = entity.CanAdd,
            CanEdit = entity.CanEdit,
            CanDelete = entity.CanDelete,
            EntitySaveMessage = entity.EntitySaveMessage
        };
    }

    /// <inheritdoc />
    public Task<PagedListMetadataDto<object>> SearchDataForEntityAsync(
        Type? entityType,
        ICollection<PropertyMetadataDto> properties,
        SearchOptions searchOptions,
        Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? searchFunction,
        Func<IServiceProvider?, IQueryable<object>, IQueryable<object>>? customQueryFunction)
    {
        if (entityType is null)
        {
            throw new NotFoundException("Entity with given type was not found.");
        }

        var query = dataService.GetQuery(entityType);

        query = SelectProperties(query, entityType, properties.Where(property => property is { IsCalculatedProperty: false, IsExcludedFromQuery: false }));

        query = ApplyCustomQuery(query, customQueryFunction);

        query = Search(query, searchOptions.SearchString, entityType, properties, searchFunction);

        if (searchOptions.OrderBy is not null)
        {
            query = Order(query, searchOptions.OrderBy.ToList(), entityType);
        }

        var pagedList = PagedListFactory.FromSource(query, searchOptions.Page, searchOptions.PageSize);

        return Task.FromResult(pagedList.ToMetadataObject());
    }

    /// <summary>
    /// Select only those properties from entity that exists in <paramref name="properties"/>.
    /// </summary>
    /// <param name="query">
    /// Query that contain data for some entity. For example, all data of <c>Address</c> entity.
    /// </param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="properties">Entity properties to select.</param>
    /// <returns>Query with selected data.</returns>
    /// <remarks>
    /// Reflection can't be translated to SQL, so we have to build expression dynamically.
    /// </remarks>
    private static IQueryable<object> SelectProperties(
        IQueryable<object> query, Type entityType, IEnumerable<PropertyMetadataDto> properties)
    {
        // entity => entity
        var entity = Expression.Parameter(typeof(object), "entity");

        // entity => (entityType)entity
        var convertedEntity = Expression.Convert(entity, entityType);

        var bindings = properties
            .Select(property => GetActualPropertyExpression(convertedEntity, property))
            .Select(member => Expression.Bind(member.Member, member));

        var ctor = entityType.GetConstructors()[0];

        // entity => new entityType
        // { PropertyName1 = ((entityType)entity).PropertyName1, PropertyName2 = ((entityType)entity).PropertyName2 ...  }
        var memberInit = Expression.MemberInit(Expression.New(ctor), bindings);

        var selectLambda = Expression.Lambda<Func<object, object>>(memberInit, entity);

        return query.Select(selectLambda);
    }

    /// <summary>
    /// Gets property expression.
    /// When <paramref name="property"/> contains inside parent class,
    /// then <paramref name="entityExpression"/> will be converted to that class.
    /// </summary>
    /// <remarks>
    /// Use case: when a parent class property has <see langword="private set"/>,
    /// then child class cannot access that <see langword="set"/>.
    /// </remarks>
    private static MemberExpression GetActualPropertyExpression(
        Expression entityExpression, PropertyMetadataDto property)
    {
        var propertyExpression = Expression.Property(entityExpression, property.Name);
        var propertyInfo = propertyExpression.Member;

        if (propertyInfo.DeclaringType == propertyInfo.ReflectedType)
        {
            return propertyExpression;
        }

        var parentEntity = Expression.Convert(entityExpression, propertyInfo.DeclaringType!);
        return Expression.Property(parentEntity, property.Name);
    }

    private IQueryable<object> Search(
        IQueryable<object> query,
        string? searchString,
        Type entityType,
        ICollection<PropertyMetadataDto> properties,
        Func<IServiceProvider?, IQueryable<object>, string, IQueryable<object>>? searchFunction)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return query;
        }

        if (properties.Any(property => property.SearchType != SearchType.None))
        {
            var propertySearches = new List<PropertySearchDto>();

            foreach (var property in properties)
            {
                if (property is NavigationMetadataDto navigation)
                {
                    foreach (var targetProperty in navigation.TargetEntityProperties)
                    {
                        propertySearches.Add(new PropertySearchDto
                        {
                            PropertyName = targetProperty.Name,
                            SearchType = targetProperty.SearchType,
                            NavigationName = navigation.Name
                        });
                    }
                }
                else
                {
                    propertySearches.Add(new PropertySearchDto
                    {
                        PropertyName = property.Name,
                        SearchType = property.SearchType
                    });
                }
            }

            query = dataService.Search(query, searchString, entityType, propertySearches);
        }

        if (searchFunction is not null)
        {
            query = searchFunction(serviceProvider, query, searchString);
        }

        return query;
    }

    private static IOrderedQueryable<object> Order(
        IQueryable<object> query, IList<OrderByDto> orderBy, Type entityType)
    {
        var orderByTuples = orderBy
            .Select(order =>
                (order.FieldName, order.IsDescending ? ListSortDirection.Descending : ListSortDirection.Ascending))
            .ToArray();

        var keySelectors = GetKeySelectors(orderBy, entityType);

        return CollectionUtils.OrderMultiple(query, orderByTuples, keySelectors);
    }

    private static (string FieldName, Expression<Func<object, object>> Selector)[] GetKeySelectors(
        IList<OrderByDto> orderByFields, Type entityType)
    {
        // entity
        var entity = Expression.Parameter(typeof(object), "entity");

        // (entityType)entity
        var convertedEntity = Expression.Convert(entity, entityType);

        // Collection of sorted properties. For example:
        // ((entityType)entity).Name
        // ((entityType)entity).Description
        // ((entityType)entity).Count
        // ((entityType)entity).Address.Street
        // ...
        // Note that there are converting property to object.
        // We need it to sort types that are not string. For example, numbers.
        var propertyExpressions = orderByFields
            .Select(field =>
            {
                var propertyName = field.NavigationName is null
                    ? field.FieldName
                    : $"{field.NavigationName}.{field.FieldName}";

                var propertyExpression = ExpressionExtensions.GetPropertyExpression(convertedEntity, propertyName);

                return Expression.Convert(propertyExpression, typeof(object));
            });

        // Make lambdas with properties. For example:
        // entity => ((entityType)entity).Name
        // entity => ((entityType)entity).Description
        // entity => ((entityType)entity).Count
        // entity => ((entityType)entity).Address.Street
        // ...
        var lambdas = propertyExpressions
            .Select(property => Expression.Lambda<Func<object, object>>(property, entity))
            .ToList();

        // Make tuple with selected property names and property lambdas. For example:
        // ("name", entity => ((entityType)entity).Name)
        // ("description", entity => ((entityType)entity).Description)
        // ("count", entity => ((entityType)entity).Count)
        // ("street", entity => ((entityType)entity).Address.Street)
        // ...
        var keySelectors = new (string, Expression<Func<object, object>>)[orderByFields.Count];
        for (var i = 0; i < orderByFields.Count; i++)
        {
            keySelectors[i] = (orderByFields[i].FieldName, lambdas[i]);
        }

        return keySelectors;
    }

    private IQueryable<object> ApplyCustomQuery(
        IQueryable<object> query,
        Func<IServiceProvider?, IQueryable<object>, IQueryable<object>>? customQueryFunction)
    {
        if (customQueryFunction is not null)
        {
            query = customQueryFunction(serviceProvider, query);
        }

        return query;
    }

    /// <inheritdoc />
    public async Task CreateEntityAsync(object entity, Type entityType, CancellationToken cancellationToken)
    {
        await dataService.AddAsync(entity, entityType, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteEntityAsync(object entity, Type entityType, CancellationToken cancellationToken)
    {
        return dataService.DeleteAsync(entity, entityType, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteEntitiesAsync(
        IEnumerable<object> entities, Type entityType, CancellationToken cancellationToken)
    {
        return dataService.BulkDeleteAsync(entities, entityType, cancellationToken);
    }

    /// <inheritdoc />
    public bool ValidateEntity(object instance, ICollection<PropertyMetadataDto> properties, ref List<ValidationResult> errors)
    {
        var context = new ValidationContext(instance, serviceProvider, items: null);

        Validator.TryValidateObject(instance, context, errors, validateAllProperties: true);

        var isNotNullableProperties = properties
            .Where(property => property is { IsNullable: false, IsReadOnly: false })
            .Select(e => e.Name)
            .ToList();

        // Validate property that not have RequiredAttribute but have RequiredMemberAttribute or is not nullable (in ORM).
        var requiredProperties = instance.GetType().GetProperties()
            .Where(prop => !prop.IsDefined(typeof(RequiredAttribute), false) &&
                           (prop.CustomAttributes.Any(attr => attr.AttributeType.Name == "RequiredMemberAttribute") ||
                            isNotNullableProperties.Contains(prop.Name)))
            .ToList();

        var requiredErrors = new List<ValidationResult>();
        foreach (var property in requiredProperties)
        {
            var value = instance.GetPropertyValue(property.Name);
            var isError = false;

            switch (value)
            {
                case string str:
                    if (string.IsNullOrEmpty(str))
                    {
                        isError = true;
                    }

                    break;
                case DateTime dt:
                    if (dt == DateTime.MinValue)
                    {
                        isError = true;
                    }

                    break;
                case DateTimeOffset dt:
                    if (dt == DateTimeOffset.MinValue)
                    {
                        isError = true;
                    }

                    break;
                case DateOnly dt:
                    if (dt == DateOnly.MinValue)
                    {
                        isError = true;
                    }

                    break;
                default:
                    if (value is null)
                    {
                        isError = true;
                    }

                    break;
            }

            if (isError)
            {
                requiredErrors.Add(new ValidationResult($"The {property.Name} field is required.", [property.Name]));
            }
        }

        if (requiredErrors.Count != 0)
        {
            errors.AddRange(requiredErrors);
        }

        return errors.Count == 0;
    }
}
