using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Saritasa.NetForge.Blazor.Constants;
using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.Services;
using Saritasa.NetForge.Blazor.Infrastructure.Authentication;
using Saritasa.NetForge.Blazor.Infrastructure.DependencyInjection.Startup;
using Saritasa.NetForge.Blazor.Infrastructure.Helpers;

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
        services.AddScoped<IAuthorizationHandler, CustomAuthFunctionHandler>();

        Infrastructure.DependencyInjection.ApplicationModule.Register(services);
    }

    /// <summary>
    /// Setup Blazor routing for the NetForge admin panel.
    /// </summary>
    /// <param name="app">Web Application instance.</param>
    public static void UseNetForge(this IApplicationBuilder app)
    {
        var optionsService = app.ApplicationServices.GetRequiredService<AdminOptions>();
        var adminPanelEndpoint = optionsService.AdminPanelEndpoint;

        // Make the application use blazor dependencies on a specific URL.
        app.MapWhen(context => context.Request.Path.StartsWithSegments(adminPanelEndpoint), applicationBuilder =>
        {
            applicationBuilder.UsePathBase(adminPanelEndpoint);
            applicationBuilder.UseStaticFiles();
            applicationBuilder.UseRouting();
            applicationBuilder.Use(AuthMiddleware);
            applicationBuilder.UseEndpoints(endpointBuilder =>
            {
                endpointBuilder.MapBlazorHub();
            });

            applicationBuilder.Run(async context =>
            {
                var result = new ViewResult
                {
                    ViewName = "_NetForge"
                };
                await context.WriteResultAsync(result);
            });
        });
    }

    private static async Task AuthMiddleware(HttpContext httpContext, Func<Task> next)
    {
        var authorizationService = httpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var isAuthorized = await authorizationService
            .AuthorizeAsync(httpContext.User, PolicyConstants.AdminAccessPolicyName);

        if (!isAuthorized.Succeeded)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await next();
    }
}
