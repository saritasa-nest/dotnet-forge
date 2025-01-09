using MudBlazor.Services;
using Saritasa.NetForge.Blazor.Domain.UseCases.Interfaces;
using Saritasa.NetForge.Blazor.Domain.UseCases.Services;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Blazor.Infrastructure.Navigation;
using Saritasa.NetForge.Blazor.Infrastructure.Services;
using Saritasa.NetForge.Blazor.MVVM.Navigation;
using Saritasa.NetForge.Blazor.MVVM.ViewModels;

namespace Saritasa.NetForge.Blazor.Infrastructure.DependencyInjection;

/// <summary>
/// Application specific dependencies.
/// </summary>
internal static class ApplicationModule
{
    /// <summary>
    /// Register dependencies.
    /// </summary>
    public static void Register(IServiceCollection services)
    {
        services.AddScoped<ViewModelFactory>();

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PreventDuplicates = false;
        });
        services.AddMemoryCache();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<IEntityService, EntityService>();
        services.AddTransient<IFileService, FileService>();
    }
}
