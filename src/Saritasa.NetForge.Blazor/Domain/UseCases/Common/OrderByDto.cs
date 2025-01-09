namespace Saritasa.NetForge.Blazor.Domain.UseCases.Common;

/// <summary>
/// DTO for ordering.
/// </summary>
public record OrderByDto
{
    /// <summary>
    /// Order by field.
    /// </summary>
    public string FieldName { get; init; } = string.Empty;

    /// <summary>
    /// Direction of order.
    /// </summary>
    public bool IsDescending { get; init; }

    /// <summary>
    /// If the order is part of navigation's property, then this property will be populated with navigation name.
    /// Otherwise, <see langword="null"/>.
    /// </summary>
    public string? NavigationName { get; init; }
}
