using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saritasa.NetForge.Blazor.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.EfCore.Extensions;
using Saritasa.NetForge.Tests.Domain;
using Xunit.Microsoft.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Saritasa.NetForge.Tests.Fixtures;

/// <summary>
/// Sets up an in-memory database and adds the NetForge library to the service collection.
/// </summary>
public class NetForgeFixture : TestBedFixture
{
    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override void AddServices(IServiceCollection services, IConfiguration? configuration)
    {
        services.AddDbContext<TestDbContext>(options => options.UseInMemoryDatabase("NetForgeTest"));

        services.AddNetForge(optionsBuilder =>
        {
            optionsBuilder.UseEntityFramework(efOptionsBuilder =>
            {
                efOptionsBuilder.UseDbContext<TestDbContext>();
            });
        });
    }

    /// <inheritdoc />
    protected override IEnumerable<TestAppSettings> GetTestAppSettings()
    {
        yield return new() { Filename = "appsettings.json", IsOptional = true };
    }
}
