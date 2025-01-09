using Microsoft.AspNetCore.Authorization;
using Saritasa.NetForge.Blazor.Constants;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Infrastructure.Authentication;

namespace Saritasa.NetForge.Blazor.Infrastructure.DependencyInjection.Startup;

/// <summary>
/// Authorization options setup for admin panel.
/// </summary>
public class AuthorizationOptionsSetup
{
    private readonly AdminOptions adminOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="adminOptions">Admin options.</param>
    public AuthorizationOptionsSetup(AdminOptions adminOptions)
    {
        this.adminOptions = adminOptions;
    }

    /// <summary>
    /// Setup authorization.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    public void Setup(AuthorizationOptions options)
    {
        options.AddPolicy(PolicyConstants.AdminAccessPolicyName, policy =>
        {
            if (!adminOptions.AdminPanelAccessRoles.Any() && adminOptions.CustomAuthFunction is null)
            {
                policy.RequireAssertion(_ => true);
            }

            if (adminOptions.AdminPanelAccessRoles.Any())
            {
                policy.RequireRole(adminOptions.AdminPanelAccessRoles);
            }

            if (adminOptions.CustomAuthFunction != null)
            {
                policy.AddRequirements(new CustomAuthFunctionRequirement(adminOptions.CustomAuthFunction));
            }
        });
    }
}
