using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;

namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Provides extension methods for assigning entity groups to the entity.
/// </summary>
internal static class EntityGroupExtensions
{
    /// <summary>
    /// Assigns the specified group to the given <see cref="EntityMetadata"/> based on the group name.
    /// </summary>
    /// <param name="entityMetadata">The metadata of the entity to which the group is being assigned.</param>
    /// <param name="groupName">The name of the group to assign to the entity.</param>
    /// <param name="adminOptions">The admin panel options containing the list of entity groups.</param>
    internal static void AssignGroupToEntity(this EntityMetadata entityMetadata, string groupName,
        AdminOptions adminOptions)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            return;
        }

        var entityGroup = adminOptions.EntityGroupsList
            .FirstOrDefault(group => group.Name == groupName);

        if (entityGroup != null)
        {
            entityMetadata.Group = entityGroup;
        }
    }
}
