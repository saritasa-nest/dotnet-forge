using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Saritasa.NetForge.Blazor.Extensions;

/// <summary>
/// Decorator to add more behavior for object.
/// </summary>
public class ObjectDecorator
{
    private readonly object innerObject;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ObjectDecorator(object innerObject)
    {
        this.innerObject = innerObject ?? throw new ArgumentNullException(nameof(innerObject));
    }

    /// <summary>
    /// Get the type of object.
    /// </summary>
    public Type GetType()
    {
        var objectType = innerObject.GetType();
        if (objectType.BaseType != null && objectType.Namespace == "Castle.Proxies")
        {
            return objectType.BaseType;
        }

        return objectType;
    }
}
