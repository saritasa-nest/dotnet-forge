using Saritasa.NetForge.Blazor.Domain.Entities.Metadata;

namespace Saritasa.NetForge.Blazor.Domain.Entities.Options;

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
    public ICollection<PropertyOptions> PropertyOptions { get; set; } = new List<PropertyOptions>();
}
