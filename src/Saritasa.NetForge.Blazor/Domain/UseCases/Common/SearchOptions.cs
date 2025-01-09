using System.ComponentModel.DataAnnotations;
using Saritasa.Tools.Common.Pagination;

namespace Saritasa.NetForge.Blazor.Domain.UseCases.Common;

/// <summary>
/// Search options that contains page and page size.
/// </summary>
public record SearchOptions
{
    /// <summary>
    /// Page number to return. Starts with 1.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = PagedList<object>.FirstPage;

    /// <summary>
    /// Required page size (amount of items returned at a time).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageSize { get; init; } = int.MaxValue;

    /// <summary>
    /// Search string.
    /// </summary>
    public string? SearchString { get; init; }

    /// <summary>
    /// Collection of order by fields with directions.
    /// </summary>
    public IEnumerable<OrderByDto>? OrderBy { get; init; }
}
