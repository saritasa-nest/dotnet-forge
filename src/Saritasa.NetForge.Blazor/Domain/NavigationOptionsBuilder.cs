using System.Linq.Expressions;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Saritasa.NetForge.Blazor.Domain.Extensions;

namespace Saritasa.NetForge.Blazor.Domain;

/// <summary>
/// Builder class for configuring entity's navigation options.
/// </summary>
public class NavigationOptionsBuilder<TEntity>
{
    private readonly NavigationOptions options = new();

    /// <summary>
    /// Creates property options.
    /// </summary>
    /// <param name="propertyName">Property name to create options for.</param>
    /// <returns>Property options.</returns>
    public NavigationOptions Create(string propertyName)
    {
        options.PropertyName = propertyName;

        return options;
    }

    /// <summary>
    /// Includes navigation's property.
    /// </summary>
    /// <param name="propertyExpression">Lambda expression representing property to include.</param>
    /// <param name="propertyOptionsBuilderAction">An action that builds property options.</param>
    public NavigationOptionsBuilder<TEntity> IncludeProperty(
        Expression<Func<TEntity, object?>> propertyExpression,
        Action<PropertyOptionsBuilder>? propertyOptionsBuilderAction = null)
    {
        var propertyOptionsBuilder = new PropertyOptionsBuilder();
        propertyOptionsBuilderAction?.Invoke(propertyOptionsBuilder);

        var propertyName = propertyExpression.GetMemberName();
        var propertyOptions = propertyOptionsBuilder.Create(propertyName);

        options.PropertyOptions.Add(propertyOptions);
        return this;
    }
}
