using System.Linq.Expressions;

namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Extensions to expressions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Gets <paramref name="expression"/> member name.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <remarks>
    /// When passed <paramref name="expression"/> like <c>entity => entity.Name</c>, then this method will return "Name".
    /// </remarks>
    /// <returns>Member name.</returns>
    public static string GetMemberName<T>(this Expression<Func<T, object?>> expression)
    {
        return GetMemberName(expression.Body);
    }

    /// <summary>
    /// Gets expression member name.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Member name.</returns>
    public static string GetMemberName(this Expression expression)
    {
        return expression switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,

            MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,

            UnaryExpression unaryExpression => GetMemberName(unaryExpression),

            null => throw new ArgumentNullException(nameof(expression), "Expression can't be null"),

            _ => throw new ArgumentException("Invalid expression.", nameof(expression))
        };
    }

    private static string GetMemberName(UnaryExpression unaryExpression)
    {
        if (unaryExpression.Operand is MethodCallExpression methodExpression)
        {
            return methodExpression.Method.Name;
        }

        return ((MemberExpression)unaryExpression.Operand).Member.Name;
    }

    /// <summary>
    /// Gets property expression that contains in <paramref name="entity"/>.
    /// To get property, just pass its name in <paramref name="propertyName"/>. For example: <c>Description</c>.
    /// This method supports getting nested properties.
    /// For example: You can pass <paramref name="propertyName"/> as <c>Address.Id</c>.
    /// </summary>
    /// <param name="entity">
    /// Entity <see cref="ParameterExpression"/>. Can have different <see cref="Expression"/> representation.
    /// For example:
    /// When you use <see cref="Expression.Convert(Expression, Type)"/> to <see cref="ParameterExpression"/>.
    /// </param>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Property <see cref="MemberExpression"/>.</returns>
    public static MemberExpression GetPropertyExpression(Expression entity, string propertyName)
    {
        var body = entity;
        foreach (var member in propertyName.Split('.'))
        {
            body = Expression.Property(body, member);
        }

        return (MemberExpression)body;
    }
}
