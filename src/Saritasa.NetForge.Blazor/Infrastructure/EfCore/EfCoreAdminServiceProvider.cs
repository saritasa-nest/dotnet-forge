using Microsoft.Extensions.DependencyInjection.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Blazor.Infrastructure.EfCore.Services;

namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore;

/// <inheritdoc />
public class EfCoreAdminServiceProvider : IAdminOrmServiceProvider
{
    private readonly EfCoreOptionsBuilder efCoreOptionsBuilder;

    /// <summary>
    /// Constructor.
    /// </summary>
    public EfCoreAdminServiceProvider(EfCoreOptionsBuilder efCoreOptionsBuilder)
    {
        this.efCoreOptionsBuilder = efCoreOptionsBuilder;
    }

    /// <inheritdoc />
    public void ApplyServices(IServiceCollection services)
    {
        var efCoreOptions = efCoreOptionsBuilder.Create();
        services.TryAddSingleton(efCoreOptions);
        services.TryAddScoped<IOrmMetadataService, EfCoreMetadataService>();
        services.TryAddScoped<IOrmDataService, EfCoreDataService>();
    }
}
