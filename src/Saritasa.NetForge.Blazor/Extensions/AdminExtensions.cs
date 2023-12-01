﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.Authentication;
using Saritasa.NetForge.Blazor.Infrastructure.DependencyInjection.Startup;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.DomainServices;
using Saritasa.NetForge.UseCases.Metadata.Services;

namespace Saritasa.NetForge.Blazor.Extensions;

/// <summary>
/// Provides extension methods to configure the admin panel.
/// </summary>
public static class AdminExtensions
{
    /// <summary>
    /// Inject required services for the NetForge.
    /// </summary>
    /// <param name="services">Collection of application services.</param>
    /// <param name="optionsBuilderAction">Action to configure the admin options using the options builder.</param>
    public static void AddNetForge(this IServiceCollection services,
        Action<AdminOptionsBuilder>? optionsBuilderAction = null)
    {
        // Build the options.
        var adminOptionsBuilder = new AdminOptionsBuilder();
        optionsBuilderAction?.Invoke(adminOptionsBuilder);

        var adminOptions = adminOptionsBuilder.Create();
        services.TryAddSingleton(adminOptions);
        adminOptionsBuilder.AdminOrmServiceProvider?.ApplyServices(services);
        services.TryAddScoped<AdminMetadataService>();

        services.Configure<AuthorizationOptions>(new AuthorizationOptionsSetup(adminOptions).Setup);
        services.AddSingleton<IAuthorizationHandler, CustomAuthFunctionHandler>();

        Infrastructure.DependencyInjection.AutoMapperModule.Register(services);
        Infrastructure.DependencyInjection.ApplicationModule.Register(services);
    }

    /// <summary>
    /// Setup Blazor routing for the NetForge admin panel.
    /// </summary>
    /// <param name="app">Web Application instance.</param>
    public static void UseNetForge(this WebApplication app)
    {
        var optionsService = app.Services.GetRequiredService<AdminOptions>();
        var adminPanelEndpoint = optionsService.AdminPanelEndpoint;

        // Make the application use blazor dependencies on a specific URL.
        app.UseWhen(context => context.Request.Path.StartsWithSegments(adminPanelEndpoint), applicationBuilder =>
        {
            applicationBuilder.UsePathBase(adminPanelEndpoint);
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseRouting();

            applicationBuilder.UseEndpoints(endpointBuilder =>
            {
                endpointBuilder.MapBlazorHub();
                endpointBuilder.MapFallbackToPage("/_NetForge");
            });
        });
    }
}
