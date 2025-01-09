using Saritasa.NetForge.Blazor.Domain.Extensions;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.Infrastructure.Helpers;

/// <summary>
/// Extensions for getting data from entity instances.
/// </summary>
public static class EntityInstanceExtensions
{
    /// <summary>
    /// Gets primary key values from given <paramref name="instance"/>.
    /// </summary>
    /// <param name="properties">Metadata of properties that <paramref name="instance"/> have.</param>
    /// <param name="instance">Entity instance.</param>
    /// <returns>Primary key values as string separated by --, e.g. <c>111--New-York--Company INC.</c></returns>
    public static string GetPrimaryKeyValues(this object instance, IEnumerable<PropertyMetadataDto> properties)
    {
        var primaryKeyValues = properties
            .Where(property => property.IsPrimaryKey)
            .Select(primaryKey => instance.GetPropertyValue(primaryKey.Name)!.ToString()!);
        return string.Join("--", primaryKeyValues);
    }
}
