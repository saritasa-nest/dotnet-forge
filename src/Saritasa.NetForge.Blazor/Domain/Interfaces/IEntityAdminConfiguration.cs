namespace Saritasa.NetForge.Blazor.Domain.Interfaces;

/// <summary>
/// Represents an interface for configuring the admin settings of a specific entity type.
/// </summary>
/// <typeparam name="TEntity">The entity type for which admin settings are being configured.</typeparam>
public interface IEntityAdminConfiguration<TEntity> where TEntity : class
{
    /// <summary>
    /// Configures the admin settings for the specified entity type.
    /// </summary>
    /// <param name="entityOptionsBuilder">A builder for configuring the admin settings of the entity type.</param>
    void Configure(EntityOptionsBuilder<TEntity> entityOptionsBuilder);
}
