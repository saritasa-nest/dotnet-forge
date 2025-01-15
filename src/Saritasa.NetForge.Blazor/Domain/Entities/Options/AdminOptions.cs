using Microsoft.AspNetCore.Components;

namespace Saritasa.NetForge.Domain.Entities.Options;

/// <summary>
/// Admin panel options.
/// </summary>
public class AdminOptions
{
    /// <summary>
    /// Admin panel endpoint.
    /// </summary>
    public string AdminPanelEndpoint { get; set; } = "/admin";

    /// <summary>
    /// Url that can be configured in admin panel.
    /// </summary>
    public string SiteUrl { get; set; } = "/";

    /// <summary>
    /// Whether the properties of entities are displayed in Title Case.
    /// By default, property names like "ContactInfo" will be displayed as "Contact Info".
    /// </summary>
    public bool TitleCaseProperties { get; set; } = true;

    /// <summary>
    /// Header title for the admin panel web page.
    /// </summary>
    public string AdminPanelHeaderTitle { get; set; } = "NetForge Admin";

    /// <summary>
    /// HTML title for the admin panel web page.
    /// </summary>
    public string AdminPanelHtmlTitle { get; set; } = "NetForge Admin - Main";

    /// <summary>
    /// Roles with access to the admin panel.
    /// </summary>
    public ICollection<string> AdminPanelAccessRoles { get; set; } = new List<string>();

    /// <summary>
    /// Custom authentication function for admin panel access.
    /// </summary>
    public Func<IServiceProvider, Task<bool>>? CustomAuthFunction { get; set; }

    /// <summary>
    /// Options for configuring entities in the admin panel.
    /// </summary>
    public ICollection<EntityOptions> EntityOptionsList { get; } = new List<EntityOptions>();

    /// <summary>
    /// Groups that entities belong to.
    /// </summary>
    public ICollection<EntityGroup> EntityGroupsList { get; } = new List<EntityGroup>();

    /// <summary>
    /// Whether entity group header should be expanded.
    /// </summary>
    public bool GroupHeadersExpanded { get; set; } = true;

    /// <summary>
    /// Path to folder that contains static files.
    /// </summary>
    public string StaticFilesFolder { get; set; } = "wwwroot";

    /// <summary>
    /// Path to folder where to store media files.
    /// </summary>
    public string MediaFolder { get; set; } = "media";

    /// <summary>
    /// Maximum image size in megabytes.
    /// </summary>
    public int MaxImageSizeInMb { get; set; } = 10;

    /// <summary>
    /// Maximum number of characters. Exceeded characters will be truncated.
    /// </summary>
    public int TruncationMaxCharacters { get; set; } = 50;

    /// <summary>
    /// Whether to include all tables from the admin panel.
    /// </summary>
    public bool IncludeAllEntities { get; set; } = true;

    /// <summary>
    /// Entities to include in the admin panel.
    /// </summary>
    public List<Type> IncludedEntities { get; set; } = [];

    /// <summary>
    /// The optional type of custom layout for the admin panel.
    /// </summary>
    public Type? CustomLayoutType { get; set; }

    /// <summary>
    /// The optional type of custom head for the admin panel.
    /// </summary>
    public Type? CustomHeadType { get; set; }

    /// <summary>
    /// Interactive content to render in the end of the body but before <see cref="StaticBodyComponentType"/>.
    /// </summary>
    public RenderFragment? InteractiveBodyContent { get; set; }

    /// <summary>
    /// Type of component that contains non-interactive content to render in the end of the body section.
    /// </summary>
    /// <remarks>
    /// We have separate static body because JS scripts have to be placed after
    /// <script src="_framework/blazor.server.js"></script> in _NetForgeLayout.cshtml.
    /// </remarks>
    public Type? StaticBodyComponentType { get; set; }
}
