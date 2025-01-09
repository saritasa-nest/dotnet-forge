using Saritasa.NetForge.Demo.Constants;
using Saritasa.NetForge.Demo.Models;
using Saritasa.NetForge.DomainServices;
using Saritasa.NetForge.DomainServices.Interfaces;

namespace Saritasa.NetForge.Demo.Infrastructure.Admin;

/// <summary>
/// <see cref="User"/> admin panel configuration.
/// </summary>
public class UserAdminConfiguration : IEntityAdminConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityOptionsBuilder<User> entityOptionsBuilder)
    {
        entityOptionsBuilder.SetGroup(GroupConstants.Identity)
            .ConfigureProperty(user => user.FullName, builder => builder.SetOrder(1))
            .ConfigureProperty(user => user.ConcurrencyStamp, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.LockoutEnd, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.NormalizedEmail, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.LockoutEnabled, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.SecurityStamp, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.NormalizedUserName, builder => builder.SetIsHidden(true))
            .ConfigureProperty(user => user.PasswordHash, builder => builder.SetIsHidden(true));
    }
}