using Saritasa.NetForge.Blazor.Domain;

namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore.Extensions;

/// <summary>
/// Provides extension methods to configure the admin panel with EF Core.
/// </summary>
public static class AdminExtensions
{
    /// <summary>
    /// Adds the EF Core handling.
    /// </summary>
    /// <param name="optionsBuilder">Admin panel options builder.</param>
    /// <param name="efCoreOptionsBuilderAction">Action to configure the EF Core options.</param>
    public static AdminOptionsBuilder UseEntityFramework(this AdminOptionsBuilder optionsBuilder,
        Action<EfCoreOptionsBuilder>? efCoreOptionsBuilderAction = null)
    {
        var efCoreOptionsBuilder = new EfCoreOptionsBuilder();
        efCoreOptionsBuilderAction?.Invoke(efCoreOptionsBuilder);
        var provider = new EfCoreAdminServiceProvider(efCoreOptionsBuilder);
        optionsBuilder.AdminOrmServiceProvider = provider;
        return optionsBuilder;
    }
}
