using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Domain.Entities.Options;

/// <summary>
/// Navigation options.
/// </summary>
public class NavigationOptions
{
    /// <inheritdoc cref="PropertyMetadataBase.Name"/>
    public string PropertyName { get; set; } = string.Empty;

    /// <inheritdoc cref="PropertyMetadataBase.Order"/>
    public int? Order { get; set; }

    /// <summary>
    /// Property options for the navigation properties.
    /// </summary>
    public ICollection<PropertyOptions> PropertyOptions { get; set; } = [];

    /// <summary>
    /// Property options for the navigation calculated properties.
    /// </summary>
    public ICollection<CalculatedPropertyOptions> CalculatedPropertyOptions { get; set; } = [];
}
