using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

/// <summary>
/// DTO for <see cref="NavigationMetadata"/>.
/// </summary>
public record NavigationMetadataDto : PropertyMetadataDto
{
    /// <inheritdoc cref="NavigationMetadata.IsCollection"/>
    public bool IsCollection { get; set; }

    /// <inheritdoc cref="NavigationMetadata.TargetEntityProperties"/>
    public List<PropertyMetadataDto> TargetEntityProperties { get; set; } = new();
}
