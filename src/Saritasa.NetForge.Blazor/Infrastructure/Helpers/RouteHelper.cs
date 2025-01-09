using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing.Template;
using Saritasa.NetForge.Blazor.Domain.Exceptions;

namespace Saritasa.NetForge.Blazor.Infrastructure.Helpers;

/// <summary>
/// Helper for component routing.
/// </summary>
public static class RouteHelper
{
    /// <summary>
    /// Get route path to component.
    /// </summary>
    /// <typeparam name="TComponent">Component.</typeparam>
    /// <returns>Route path.</returns>
    public static string GetRoute<TComponent>()
        where TComponent : ComponentBase
    {
        var componentType = typeof(TComponent);
        return GetRoute(componentType);
    }

    /// <summary>
    /// Get route path to component.
    /// </summary>
    /// <param name="componentType">Component type.</param>
    /// <returns>Route path.</returns>
    public static string GetRoute(Type componentType)
    {
        var routeTemplate = RouteTemplateHelper.GetRouteTemplate(componentType);

        if (routeTemplate.Parameters.Any())
        {
            throw new InvalidOperationException($"Need call {nameof(GetRoute)} method with parameters.");
        }

        return routeTemplate.TemplateText ?? string.Empty;
    }

    /// <summary>
    /// Get route path to component with parameters.
    /// </summary>
    /// <typeparam name="TComponent">Component.</typeparam>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    public static string GetRoute<TComponent>(IReadOnlyDictionary<string, string> parameters)
        where TComponent : ComponentBase
    {
        var routeTemplate = RouteTemplateHelper.GetRouteTemplate<TComponent>();
        return GetRoute(routeTemplate, parameters);
    }

    /// <summary>
    /// Get route path to component with parameters.
    /// </summary>
    /// <typeparam name="TComponent">Component.</typeparam>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    public static string GetRoute<TComponent>(params object[] parameters)
        where TComponent : ComponentBase
    {
        var routeTemplate = RouteTemplateHelper.GetRouteTemplate<TComponent>();
        return GetRoute(routeTemplate, parameters);
    }

    /// <summary>
    /// Get route path to component with parameters.
    /// </summary>
    /// <param name="componentType">Component type.</param>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    public static string GetRoute(Type componentType, IReadOnlyDictionary<string, string> parameters)
    {
        var routeTemplate = RouteTemplateHelper.GetRouteTemplate(componentType);
        return GetRoute(routeTemplate, parameters);
    }

    /// <summary>
    /// Get route path to component with parameters.
    /// </summary>
    /// <param name="componentType">Component type.</param>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    public static string GetRoute(Type componentType, params object[] parameters)
    {
        var routeTemplate = RouteTemplateHelper.GetRouteTemplate(componentType);
        return GetRoute(routeTemplate, parameters);
    }

    /// <summary>
    /// Get route path with parameters.
    /// </summary>
    /// <param name="routeTemplate">Route template.</param>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    private static string GetRoute(RouteTemplate routeTemplate, params object[] parameters)
    {
        var parametersKeyValue = GetParametersDictionary(routeTemplate, parameters);
        return GetRoute(routeTemplate, parametersKeyValue);
    }

    /// <summary>
    /// Get route path with parameters.
    /// </summary>
    /// <param name="routeTemplate">Route template.</param>
    /// <param name="parameters">Parameters for route.</param>
    /// <returns>Route path.</returns>
    private static string GetRoute(RouteTemplate routeTemplate, IReadOnlyDictionary<string, string> parameters)
    {
        var urlParts = new List<string>();

        foreach (var segment in routeTemplate.Segments)
        {
            var part = segment.Parts.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(part?.Name))
            {
                urlParts.Add(part?.Text ?? string.Empty);
                continue;
            }

            if (!parameters.TryGetValue(part.Name, out var value))
            {
                throw new DomainException($"No value found for \"{part.Name}\" parameter.");
            }

            urlParts.Add(value);
        }

        return string.Join("/", urlParts);
    }

    /// <summary>
    /// Converts route parameters into a dictionary for route template substitution.
    /// </summary>
    /// <param name="routeTemplate">The route template containing parameter names.</param>
    /// <param name="parameters">The parameters to substitute into the route template.</param>
    /// <returns>A dictionary mapping parameter names to their corresponding values.</returns>
    /// <exception cref="ArgumentException">Thrown when the count of passed parameters does not match the count
    /// of parameters in the template.</exception>
    private static IReadOnlyDictionary<string, string> GetParametersDictionary(RouteTemplate routeTemplate,
        params object[] parameters)
    {
        if (routeTemplate.Parameters.Count != parameters.Length)
        {
            throw new ArgumentException("Count passed parameters does not match count parameters in template.");
        }

        var parametersDictionary = new Dictionary<string, string>();

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i].ToString() ?? string.Empty;
            var parameterName = routeTemplate.Parameters[i].Name ?? string.Empty;
            parametersDictionary.TryAdd(parameterName, parameter);
        }

        return parametersDictionary;
    }
}
