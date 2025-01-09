using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Demo.Infrastructure.Startup;
using Saritasa.NetForge.Demo.Infrastructure.Startup.HealthCheck;
using Saritasa.NetForge.Demo.Infrastructure.UploadFiles.S3Storage;
using Saritasa.NetForge.Demo.Models;

namespace Saritasa.NetForge.Demo;

/// <summary>
/// Entry point for ASP.NET Core app.
/// </summary>
public class Startup
{
    private readonly IConfiguration configuration;

    /// <summary>
    /// Entry point for web application.
    /// </summary>
    /// <param name="configuration">Global configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// Configure application services on startup.
    /// </summary>
    /// <param name="services">Services to configure.</param>
    /// <param name="environment">Application environment.</param>
    public void ConfigureServices(IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddRazorPages();
        var connectionString = configuration.GetConnectionString("AppDatabase")
                               ?? throw new ArgumentNullException("ConnectionStrings:AppDatabase",
                                   "Database connection string is not initialized");

        services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        services.AddAsyncInitializer<DatabaseInitializer>();
        services.AddHealthChecks().AddNpgSql(connectionString);

        // Identity.
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ShopDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<IdentityOptions>(new IdentityOptionsSetup().Setup);

        // S3 storage
        services.Configure<S3Settings>(configuration.GetSection("S3Settings"));

        services.AddScoped<IBlobStorageService, S3StorageService>();
        services.AddScoped<ICloudBlobStorageService, S3StorageService>();

        // Add NetForge admin panel.
        Infrastructure.DependencyInjection.NetForgeModule.Register(services, configuration);
    }

    /// <summary>
    /// Configure web application.
    /// </summary>
    /// <param name="app">Application builder.</param>
    public void Configure(WebApplication app)
    {
        HealthCheckModule.Register(app);

        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseNetForge();

        var cultureInfo = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }
}