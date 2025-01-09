namespace Saritasa.NetForge.Blazor.Infrastructure.EfCore;

/// <summary>
/// EF Core options for the admin panel.
/// </summary>
public class EfCoreOptions
{
    /// <summary>
    /// Collection of EF Core DB contexts need to be used in panel.
    /// </summary>
    public ICollection<Type> DbContexts { get; } = new List<Type>();
}
