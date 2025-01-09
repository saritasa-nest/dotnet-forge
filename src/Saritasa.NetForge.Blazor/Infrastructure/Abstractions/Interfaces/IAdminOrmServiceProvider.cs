namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Represents a contract for injecting ORM-specific services into the admin panel.
/// </summary>
public interface IAdminOrmServiceProvider
{
    /// <summary>
    /// Injects ORM-specific services into the admin panel.
    /// </summary>
    /// <param name="services">Collection of application services.</param>
    void ApplyServices(IServiceCollection services);
}
