namespace Saritasa.NetForge.Domain.Entities.Options;

/// <summary>
/// Options for entity callbacks that can be executed during various entity lifecycle events.
/// </summary>
public class EntityCallbackOptions
{
    /// <summary>
    /// Callback to be executed before an entity is created.
    /// </summary>
    public Func<CancellationToken, Task>? PreCreate { get; set; } = null;

    /// <summary>
    /// Callback to be executed before an entity is edited.
    /// </summary>
    public Func<CancellationToken, Task>? PreEdit { get; set; } = null;

    /// <summary>
    /// Callback to be executed before an entity is deleted.
    /// </summary>
    public Func<CancellationToken, Task>? PreDelete { get; set; } = null;
}
