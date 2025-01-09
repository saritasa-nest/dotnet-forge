using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Domain.Comparers;

namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore.Extensions;

/// <summary>
/// Database context extensions.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Get <see cref="DbSet{TEntity}"/> of given <paramref name="entityType"/>.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="entityType">Entity type.</param>
    /// <returns><see cref="IQueryable"/> that populated with entities of <paramref name="entityType"/>.</returns>
    /// <remarks>
    /// Due to absence of non generic version of <see cref="DbContext.Set{TEntity}()"/> we created this method.
    /// </remarks>
    public static IQueryable Set(this DbContext context, Type entityType)
    {
        var setMethod = typeof(DbContext)
            .GetMethods()
            .Single(p => p is { Name: nameof(DbContext.Set), ContainsGenericParameters: true } &&
                         !p.GetParameters().Any());

        setMethod = setMethod.MakeGenericMethod(entityType);

        return (setMethod.Invoke(context, null) as IQueryable)!;
    }

    /// <summary>
    /// Determine is <paramref name="entity"/> tracked by <see cref="DbContext.ChangeTracker"/>.
    /// </summary>
    /// <param name="context">Database context.</param>
    /// <param name="entity">Entity type.</param>
    /// <returns>
    /// When <paramref name="entity"/> tracked - <see langword="true"/>, otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsTracked(this DbContext context, object entity)
    {
        return context.ChangeTracker
            .Entries()
            .Select(entry => entry.Entity)
            .Contains(entity, new ObjectComparer<object>());
    }
}
