namespace Saritasa.NetForge.Blazor.Domain.Enums;

/// <summary>
/// Search type.
/// </summary>
public enum SearchType
{
    /// <summary>
    /// Case insensitive partial search.
    /// </summary>
    ContainsCaseInsensitive,

    /// <summary>
    /// Case sensitive starts with search.
    /// </summary>
    StartsWithCaseSensitive,

    /// <summary>
    /// Case insensitive exact match search.
    /// </summary>
    ExactMatchCaseInsensitive,

    /// <summary>
    /// Search type is not specified.
    /// </summary>
    None
}
