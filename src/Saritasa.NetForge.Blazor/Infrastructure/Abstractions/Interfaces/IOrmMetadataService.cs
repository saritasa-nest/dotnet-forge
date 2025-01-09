using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;

/// <summary>
/// Provides a contract for retrieving ORM-specific metadata for entities.
/// </summary>
public interface IOrmMetadataService
{
    /// <summary>
    /// Get metadata for entities.
    /// </summary>
    IEnumerable<EntityMetadata> GetMetadata();
}
