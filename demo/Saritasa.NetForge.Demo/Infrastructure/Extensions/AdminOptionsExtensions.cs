using Microsoft.AspNetCore.Identity;
using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Demo.Constants;

namespace Saritasa.NetForge.Demo.Infrastructure.Extensions;

/// <summary>
/// Extension class for <see cref="AdminOptionsBuilder"/>.
/// </summary>
public static class AdminOptionsExtensions
{
    /// <summary>
    /// Add entities into identity group.
    /// </summary>
    /// <param name="optionsBuilder">Admin options builder.</param>
    public static AdminOptionsBuilder AddIdentityGroup(this AdminOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureEntity<IdentityRole>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetGroup(GroupConstants.Identity);
        }).ConfigureEntity<IdentityRoleClaim<string>>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetIsHidden(true);
        }).ConfigureEntity<IdentityUserClaim<string>>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetIsHidden(true);
        }).ConfigureEntity<IdentityUserLogin<string>>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetIsHidden(true);
        }).ConfigureEntity<IdentityUserRole<string>>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetIsHidden(true);
        }).ConfigureEntity<IdentityUserToken<string>>(entityOptionsBuilder =>
        {
            entityOptionsBuilder.SetIsHidden(true);
        });

        return optionsBuilder;
    }
}