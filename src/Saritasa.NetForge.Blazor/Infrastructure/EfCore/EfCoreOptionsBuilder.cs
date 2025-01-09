using Microsoft.EntityFrameworkCore;

namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore;

/// <summary>
/// Builds EF Core specific options for the admin panel.
/// </summary>
public class EfCoreOptionsBuilder
{
    /// <summary>
    /// EF Core specific options.
    /// </summary>
    private readonly EfCoreOptions options = new();

    /// <summary>
    /// Adds DB context to use in the panel.
    /// </summary>
    /// <typeparam name="TDbContext">Type of EF Core DB context.</typeparam>
    /// <returns>The instance of the options builder.</returns>
    public EfCoreOptionsBuilder UseDbContext<TDbContext>() where TDbContext : DbContext
    {
        options.DbContexts.Add(typeof(TDbContext));
        return this;
    }

    /// <summary>
    /// Get EF Core specific options for the admin panel.
    /// </summary>
    public EfCoreOptions Create()
    {
        return options;
    }
}
