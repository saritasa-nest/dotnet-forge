using Saritasa.NetForge.Blazor.Domain.Dtos;

namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Service for retrieving data from ORM.
/// </summary>
public interface IOrmDataService
{
    /// <summary>
    /// Get query of entity with specified type.
    /// </summary>
    /// <param name="clrType">CLR type.</param>
    /// <returns>Entity data.</returns>
    IQueryable<object> GetQuery(Type clrType);

    /// <summary>
    /// Get instance by primary key value.
    /// </summary>
    /// <param name="primaryKey">
    /// Primary key values.
    /// In case of composite primary key, they have to be separated by "--".
    /// </param>
    /// <param name="entityType">Type to get instance of.</param>
    /// <param name="includedNavigationNames">Included navigation names.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Instance.</returns>
    Task<object> GetInstanceAsync(
        string primaryKey,
        Type entityType,
        IEnumerable<string> includedNavigationNames,
        CancellationToken cancellationToken);

    /// <summary>
    /// Performs search.
    /// </summary>
    /// <param name="query">Query to search.</param>
    /// <param name="searchString">Search string.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="properties">Properties.</param>
    /// <returns>Query with searched data.</returns>
    IQueryable<object> Search(
        IQueryable<object> query,
        string searchString,
        Type entityType,
        IEnumerable<PropertySearchDto> properties);

    /// <summary>
    /// Adds entity to the database.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task AddAsync(object entity, Type entityType, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes entity in the database.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task DeleteAsync(object entity, Type entityType, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes all provided entities in the database.
    /// </summary>
    /// <param name="entities">Entities to delete.</param>
    /// <param name="entityType">Entity type.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task BulkDeleteAsync(IEnumerable<object> entities, Type entityType, CancellationToken cancellationToken);

    /// <summary>
    /// Updates entity in the database.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <param name="originalEntity">Entity that contains initial unchanged values.</param>
    /// <param name="afterUpdateAction">Action that will be called after update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    Task<object> UpdateAsync(
        object entity,
        object originalEntity,
        Action<IServiceProvider?, object, object>? afterUpdateAction,
        CancellationToken cancellationToken);
}
