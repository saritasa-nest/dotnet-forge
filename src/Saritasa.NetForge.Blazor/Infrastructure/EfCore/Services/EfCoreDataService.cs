using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Saritasa.NetForge.Blazor.Domain.Comparers;
using Saritasa.NetForge.Blazor.Domain.Dtos;
using Saritasa.NetForge.Blazor.Domain.Enums;
using Saritasa.NetForge.Blazor.Domain.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Blazor.Infrastructure.EfCore.Extensions;
using ExpressionExtensions = Saritasa.NetForge.Blazor.Domain.Extensions.ExpressionExtensions;

namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore.Services;

/// <summary>
/// Data service for EF core.
/// </summary>
public class EfCoreDataService : IOrmDataService
{
    private const string Entity = "entity";

    private readonly EfCoreOptions efCoreOptions;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EfCoreDataService(EfCoreOptions efCoreOptions, IServiceProvider serviceProvider)
    {
        this.efCoreOptions = efCoreOptions;
        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public IQueryable<object> GetQuery(Type clrType)
    {
        var dbContext = GetDbContextThatContainsEntity(clrType);
        return dbContext.Set(clrType).OfType<object>().AsNoTracking();
    }

    /// <inheritdoc />
    public async Task<object> GetInstanceAsync(
        string primaryKey,
        Type entityType,
        IEnumerable<string> includedNavigationNames,
        CancellationToken cancellationToken)
    {
        var dbContext = GetDbContextThatContainsEntity(entityType);
        var type = dbContext.Model.FindEntityType(entityType)!;
        var key = type.FindPrimaryKey()!;

        var primaryKeyNames = key.Properties.Select(property => property.Name);
        var primaryKeyValues = primaryKey.Split("--");
        var primaryKeyNamesWithValues = primaryKeyNames.Zip(primaryKeyValues);

        var query = GetQuery(entityType);

        // entity
        var entity = Expression.Parameter(typeof(object), Entity);

        // (entityType)entity
        var convertedEntity = Expression.Convert(entity, entityType);

        Expression? primaryKeyExpression = null;
        foreach (var (name, value) in primaryKeyNamesWithValues)
        {
            // ((entityType)entity).propertyName
            var propertyExpression = Expression.Property(convertedEntity, name);

            var property = GetConvertedExpressionWhenPropertyIsNotString(propertyExpression);
            var constant = Expression.Constant(value);

            // ((entityType)entity).propertyName.StartsWith(constant)
            var equalsCall = Expression.Call(
                property, typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(object) })!, constant);

            primaryKeyExpression = primaryKeyExpression is null
                ? equalsCall
                : AddAndBetweenExpressions(equalsCall, primaryKeyExpression);
        }

        // Example with composite primary key:
        // entity => ((entityType)entity).propertyName1.StartsWith(constant1)
        // && ((entityType)entity).propertyName2.StartsWith(constant2)
        var lambda = Expression.Lambda<Func<object, bool>>(primaryKeyExpression!, entity);

        foreach (var navigationName in includedNavigationNames)
        {
            var navigationExpression = ExpressionExtensions.GetPropertyExpression(convertedEntity, navigationName);
            var navigationLambda = Expression.Lambda<Func<object, object>>(navigationExpression, entity);

            query = query.Include(navigationLambda);
        }

        return await query.FirstAsync(lambda, cancellationToken);
    }

    private DbContext GetDbContextThatContainsEntity(Type clrType)
    {
        foreach (var dbContextType in efCoreOptions.DbContexts)
        {
            var dbContextService = serviceProvider.GetService(dbContextType);

            if (dbContextService == null)
            {
                continue;
            }

            var dbContext = (DbContext)dbContextService;
            var entityType = dbContext.Model.FindEntityType(clrType);

            if (entityType is not null)
            {
                return dbContext;
            }
        }

        throw new ArgumentException("Database entity with given type was not found", nameof(clrType));
    }

    /// <inheritdoc />
    public IQueryable<object> Search(
        IQueryable<object> query,
        string searchString,
        Type entityType,
        IEnumerable<PropertySearchDto> properties)
    {
        // entity => entity
        var entity = Expression.Parameter(typeof(object), Entity);

        // entity => (entityType)entity
        var convertedEntity = Expression.Convert(entity, entityType);

        Expression? combinedSearchExpressions = null;

        var searchEntries = GetSearchEntries(searchString);
        foreach (var searchEntry in searchEntries)
        {
            var singleEntrySearchExpression = GetEntrySearchExpression(properties, convertedEntity, searchEntry);

            combinedSearchExpressions =
                AddAndBetweenExpressions(combinedSearchExpressions, singleEntrySearchExpression);
        }

        if (combinedSearchExpressions is null)
        {
            return query;
        }

        var predicate = Expression.Lambda<Func<object, bool>>(combinedSearchExpressions, entity);
        return query.Where(predicate);
    }

    /// <summary>
    /// Applies search using search entry to every searchable property, every property can have their own search type.
    /// </summary>
    private static Expression GetEntrySearchExpression(
        IEnumerable<PropertySearchDto> properties,
        Expression entity,
        string searchEntry)
    {
        Expression? singleEntrySearchExpression = null;
        foreach (var property in properties)
        {
            if (property.SearchType == SearchType.None)
            {
                continue;
            }

            var propertyName = property.NavigationName is null
                ? property.PropertyName
                : $"{property.NavigationName}.{property.PropertyName}";

            var propertyExpression = ExpressionExtensions.GetPropertyExpression(entity, propertyName);

            var searchMethodCallExpression = property.SearchType switch
            {
                SearchType.ContainsCaseInsensitive
                    => GetContainsCaseInsensitiveMethodCall(propertyExpression, searchEntry),

                SearchType.StartsWithCaseSensitive
                    => GetStartsWithCaseSensitiveMethodCall(propertyExpression, searchEntry),

                SearchType.ExactMatchCaseInsensitive
                    => GetExactMatchCaseInsensitiveMethodCall(propertyExpression, searchEntry),

                _ => throw new InvalidOperationException("Incorrect search type was used.")
            };

            singleEntrySearchExpression =
                AddOrBetweenSearchExpressions(singleEntrySearchExpression, searchMethodCallExpression);
        }

        return singleEntrySearchExpression!;
    }

    /// <summary>
    /// Combines all expressions to one expression with <see langword="OR"/> operator between.
    /// </summary>
    private static Expression AddOrBetweenSearchExpressions(
        Expression? combinedExpressions, Expression expression)
    {
        if (combinedExpressions is not null)
        {
            // Add OR operator between every searchable property using search entry
            // Example:
            // entity => ((entityType)entity).propertyName.Equals(searchEntry) ||
            //           ((entityType)entity).propertyName2.StartsWith(searchEntry) ||
            //           ...
            return Expression.OrElse(combinedExpressions, expression);
        }

        return expression;
    }

    /// <summary>
    /// Combines all expressions to one expression with <see langword="AND"/> operator between.
    /// </summary>
    private static Expression AddAndBetweenExpressions(
        Expression? combinedExpressions, Expression expression)
    {
        if (combinedExpressions is not null)
        {
            // Example:
            // entity => ((entityType)entity).propertyName.Equals(searchEntry) ||
            //           ((entityType)entity).propertyName2.StartsWith(searchEntry) ||
            //           ...) &&
            //           ((entityType)entity).propertyName.Equals(searchEntry2) ||
            //           ((entityType)entity).propertyName2.StartsWith(searchEntry2) ||
            //           ...) && ...
            return Expression.And(combinedExpressions, expression);
        }

        return expression;
    }

    /// <summary>
    /// Retrieves search entries from <paramref name="searchString"/> using regular expression. Handles single and double quotes.
    /// </summary>
    /// <param name="searchString">Search string.</param>
    /// <returns>Collection of search entries.</returns>
    /// <remarks>
    /// For example if search string is: <c>"Double quotes" 'Single quotes' Without quotes</c>,
    /// then result will have these entries:
    /// <list type="bullet">
    ///     <item>Double quotes</item>
    ///     <item>Single quotes</item>
    ///     <item>Without</item>
    ///     <item>quotes</item>
    /// </list>
    /// </remarks>
    private static IEnumerable<string> GetSearchEntries(string searchString)
    {
        const string splitSearchStringRegex = """(?<=["])[^"]*(?="\s|"$)|(?<=['])[^']*(?='\s|'$)|[^\s"']+""";
        var matches = Regex.Matches(searchString, splitSearchStringRegex);

        return matches.Select(match => match.Value);
    }

    /// <summary>
    /// Gets call of method similar to <see cref="string.Contains(string)"/> but case insensitive.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="string.ToUpper()"/> to achieve case insensitive search.
    /// </remarks>
    private static Expression GetContainsCaseInsensitiveMethodCall(
        MemberExpression propertyExpression, string searchEntry)
    {
        var property = GetConvertedExpressionWhenPropertyIsNotString(propertyExpression);
        var entryConstant = Expression.Constant(searchEntry);

        Expression<Func<string, string, bool>> containsExpression =
            (property, value) => property.ToUpper().Contains(value.ToUpper());

        var body = containsExpression.Body;

        body = ReplacingExpressionVisitor.Replace(containsExpression.Parameters[0], property, body);
        body = ReplacingExpressionVisitor.Replace(containsExpression.Parameters[1], entryConstant, body);

        return body;
    }

    /// <summary>
    /// Gets call of <see cref="string.StartsWith(string)"/>.
    /// </summary>
    /// <remarks>
    /// If given search entry is not string, <c>ToString</c> will be called.
    /// </remarks>
    private static Expression GetStartsWithCaseSensitiveMethodCall(
        MemberExpression propertyExpression, string searchEntry)
    {
        var property = GetConvertedExpressionWhenPropertyIsNotString(propertyExpression);
        var startsWith = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) })!;
        var entryConstant = Expression.Constant(searchEntry);

        // entity => ((entityType)entity).propertyName.StartsWith(searchConstant)
        return Expression.Call(property, startsWith, entryConstant);
    }

    /// <summary>
    /// Gets call of method similar to <see cref="string.Equals(string)"/> but case insensitive.
    /// If provided search entry is <c>None</c>, then this method will perform <c>IS NULL</c> check.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="string.ToUpper()"/> to achieve case insensitive search.
    /// </remarks>
    private static Expression GetExactMatchCaseInsensitiveMethodCall(
        MemberExpression propertyExpression, string searchEntry)
    {
        if (searchEntry.Equals("None"))
        {
            return GetNullCheckExpression(propertyExpression);
        }

        var property = GetConvertedExpressionWhenPropertyIsNotString(propertyExpression);

        Expression<Func<string, string, bool>> equalsExpression =
            (property, value) => property.ToUpper().Equals(value.ToUpper());

        var body = equalsExpression.Body;

        body = ReplacingExpressionVisitor.Replace(equalsExpression.Parameters[0], property, body);

        var entryConstant = Expression.Constant(searchEntry);
        body = ReplacingExpressionVisitor.Replace(equalsExpression.Parameters[1], entryConstant, body);

        return body;
    }

    /// <summary>
    /// Gets equal expression where <paramref name="expression"/> will be compared to <see langword="null"/>.
    /// </summary>
    /// <param name="expression">Expression to check.</param>
    private static Expression GetNullCheckExpression(Expression expression)
    {
        var nullConstant = Expression.Constant(null, typeof(object));
        return Expression.Equal(expression, nullConstant);
    }

    /// <summary>
    /// When <paramref name="propertyExpression"/> does not represent <see langword="string"/>
    /// then <c>ToString</c> will be called to underlying property.
    /// </summary>
    private static Expression GetConvertedExpressionWhenPropertyIsNotString(MemberExpression propertyExpression)
    {
        var propertyType = ((PropertyInfo)propertyExpression.Member).PropertyType;

        if (propertyType != typeof(string))
        {
            return Expression.Call(propertyExpression, typeof(object).GetMethod(nameof(ToString))!);
        }

        return propertyExpression;
    }

    /// <inheritdoc />
    public async Task AddAsync(object entity, Type entityType, CancellationToken cancellationToken)
    {
        var dbContext = GetDbContextThatContainsEntity(entityType);
        try
        {
            // We use Attach instead of Add because
            // EF will try to create new entity and create all navigation (even when they are exist in database).
            // Attach resolves this problem by explicitly attaching navigation to EF change tracker.
            dbContext.Attach(entity);
            dbContext.Add(entity);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            // Need to clear because when we re-add the same entity
            // EF will throw an exception that entity is already tracked.
            dbContext.ChangeTracker.Clear();

            throw;
        }

        // Since we use different dbContext instances for operations, an error can occur if a dbContext
        // instance is used before the last dbContext instance is disposed. For example: If a user creates
        // a record and deletes it immediately, there will be an error because within the same viewmodel
        // lifecycle, there are two instances of dbContext modifying the same record. Create and update
        // operations are not affected by this because they have their own viewmodels, allowing them to
        // create new instances of dbContext. However, the delete function does not have its own viewmodel.
        // Therefore, after adding a new model, we need to clear the tracking for the record so that the
        // delete operation can work.
        dbContext.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task DeleteAsync(object entity, Type entityType, CancellationToken cancellationToken)
    {
        var dbContext = GetDbContextThatContainsEntity(entityType);

        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task BulkDeleteAsync(
        IEnumerable<object> entities, Type entityType, CancellationToken cancellationToken)
    {
        var dbContext = GetDbContextThatContainsEntity(entityType);

        foreach (var entity in entities)
        {
            dbContext.Remove(entity);
        }
        await dbContext.SaveChangesAsync(cancellationToken);

        dbContext.ChangeTracker.Clear();
    }

    /// <inheritdoc />
    public async Task<object> UpdateAsync(
        object entity,
        object originalEntity,
        Action<IServiceProvider?, object, object>? afterUpdateAction,
        CancellationToken cancellationToken)
    {
        var entityType = entity.GetType();
        var dbContext = GetDbContextThatContainsEntity(entityType);

        if (afterUpdateAction is not null)
        {
            var originalEntityClone = originalEntity.CloneJson();
            await UpdateAsync(dbContext, entity, originalEntity, cancellationToken);

            afterUpdateAction.Invoke(serviceProvider, originalEntityClone!, originalEntity);
        }
        else
        {
            await UpdateAsync(dbContext, entity, originalEntity, cancellationToken);
        }

        dbContext.ChangeTracker.Clear();
        return originalEntity;
    }

    private static async Task UpdateAsync(
        DbContext dbContext, object entity, object originalEntity, CancellationToken cancellationToken)
    {
        dbContext.Attach(originalEntity);

        UpdateNavigations(dbContext, entity, originalEntity);

        dbContext.Entry(originalEntity).CurrentValues.SetValues(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void UpdateNavigations(DbContext dbContext, object entity, object originalEntity)
    {
        // By default, EF does not track removed items from navigation collections
        // when you just change reference to another collection.
        // We use this foreach to explicitly remove items from navigation collection
        // to give EF opportunity to track these changes.
        foreach (var navigationEntry in dbContext.Entry(entity).Navigations)
        {
            var originalNavigationEntry = dbContext
                .Entry(originalEntity)
                .Navigation(navigationEntry.Metadata.Name);

            if (navigationEntry.CurrentValue is null && originalNavigationEntry.CurrentValue is null)
            {
                continue;
            }

            if (!navigationEntry.Metadata.IsCollection)
            {
                UpdateNavigationReference(dbContext, navigationEntry, originalNavigationEntry);

                continue;
            }

            UpdateNavigationCollection(dbContext, originalNavigationEntry, navigationEntry);
        }
    }

    private static void UpdateNavigationReference(
        DbContext dbContext, NavigationEntry navigationEntry, NavigationEntry originalNavigationEntry)
    {
        // Case when the user want to remove navigation value
        if (navigationEntry.CurrentValue is null && originalNavigationEntry.CurrentValue is not null)
        {
            originalNavigationEntry.CurrentValue = navigationEntry.CurrentValue;
        }
        else
        {
            var isTracked = dbContext.IsTracked(navigationEntry.CurrentValue!);

            if (!isTracked)
            {
                dbContext.Attach(navigationEntry.CurrentValue!);
                originalNavigationEntry.CurrentValue = navigationEntry.CurrentValue;
            }
        }
    }

    private static void UpdateNavigationCollection(
        DbContext dbContext, NavigationEntry originalNavigationEntry, NavigationEntry navigationEntry)
    {
        var navigationCollectionInstance = (IEnumerable<object>)navigationEntry.CurrentValue!;

        // Track added elements
        foreach (var element in navigationCollectionInstance)
        {
            var isTracked = dbContext.IsTracked(element);

            if (!isTracked)
            {
                dbContext.Attach(element);
            }
        }

        var originalNavigationCollectionInstance = (IEnumerable<object>)originalNavigationEntry.CurrentValue!;

        var objectComparer = new ObjectComparer<object>();

        var addedElements = navigationCollectionInstance
            .Except(originalNavigationCollectionInstance, objectComparer);

        var removedElements = originalNavigationCollectionInstance
            .Except(navigationCollectionInstance, objectComparer);

        var actualElements = originalNavigationCollectionInstance
            .Union(addedElements)
            .Except(removedElements);

        var underlyingTypeOfCollection = navigationEntry.Metadata.TargetEntityType.ClrType;

        var castCollection = actualElements.Cast(underlyingTypeOfCollection);

        originalNavigationEntry.CurrentValue = typeof(Enumerable)
            .GetMethod(nameof(Enumerable.ToList))!
            .MakeGenericMethod(underlyingTypeOfCollection)
            .Invoke(null, [castCollection]);
    }
}
