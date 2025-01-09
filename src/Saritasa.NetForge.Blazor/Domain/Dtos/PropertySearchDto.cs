using Saritasa.NetForge.Blazor.Domain.Enums;

namespace Saritasa.NetForge.Blazor.Domain.Dtos;

/// <summary>
/// Property search DTO.
/// </summary>
public record PropertySearchDto
{
    /// <summary>
    /// Property name.
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Search type.
    /// </summary>
    public SearchType SearchType { get; set; }

    /// <summary>
    /// Navigation name. <see langword="null"/> if the property is not related to navigation.
    /// </summary>
    public string? NavigationName { get; set; }
}
