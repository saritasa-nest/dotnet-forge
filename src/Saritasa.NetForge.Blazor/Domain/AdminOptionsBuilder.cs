using Saritasa.NetForge.Domain.Attributes;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.DomainServices.Interfaces;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;

namespace Saritasa.NetForge.DomainServices;

/// <summary>
/// Builds the admin options.
/// </summary>
public class AdminOptionsBuilder
{
    private readonly AdminOptions options = new();

    /// <summary>
    /// Provider for injecting ORM-specific services.
    /// </summary>
    public IAdminOrmServiceProvider? AdminOrmServiceProvider { get; set; }

    /// <summary>
    /// Set the URL on which the admin panel will be hosted.
    /// </summary>
    /// <param name="url">URL.</param>
    public AdminOptionsBuilder UseEndpoint(string url)
    {
        options.AdminPanelEndpoint = url;
        return this;
    }

    /// <summary>
    /// Set the header title for the admin panel web page.
    /// </summary>
    /// <param name="title">Header title.</param>
    public AdminOptionsBuilder SetHeaderTitle(string title)
    {
        options.AdminPanelHeaderTitle = title;
        return this;
    }

    /// <summary>
    /// Set the html title for the admin panel web page.
    /// </summary>
    /// <param name="title">Html title.</param>
    public AdminOptionsBuilder SetHtmlTitle(string title)
    {
        options.AdminPanelHtmlTitle = title;
        return this;
    }

    /// <summary>
    /// Adds roles that have access to the admin panel.
    /// </summary>
    /// <param name="roles">Roles to be added for access.</param>
    public AdminOptionsBuilder AddAccessRoles(params string[] roles)
    {
        if (!roles.Any())
        {
            return this;
        }

        foreach (var role in roles)
        {
            options.AdminPanelAccessRoles.Add(role);
        }

        options.AdminPanelAccessRoles = options.AdminPanelAccessRoles.Distinct().ToList();
        return this;
    }

    /// <summary>
    /// Configures custom authentication for the admin module.
    /// </summary>
    /// <param name="customAuthFunction">Custom authenticate function.</param>
    public AdminOptionsBuilder ConfigureAuth(Func<IServiceProvider, Task<bool>> customAuthFunction)
    {
        options.CustomAuthFunction = customAuthFunction;
        return this;
    }

    /// <summary>
    /// Get options for the admin panel.
    /// </summary>
    public AdminOptions Create()
    {
        return options;
    }

    /// <summary>
    /// Configures options for a specific entity type within the admin panel.
    /// </summary>
    /// <param name="entityOptionsBuilderAction">An action that builds entity options.</param>
    /// <typeparam name="TEntity">The type of entity for which options are being configured.</typeparam>
    public AdminOptionsBuilder ConfigureEntity<TEntity>(
        Action<EntityOptionsBuilder<TEntity>> entityOptionsBuilderAction) where TEntity : class
    {
        var entityOptionsBuilder = new EntityOptionsBuilder<TEntity>();
        entityOptionsBuilderAction.Invoke(entityOptionsBuilder);
        AddOrUpdateEntityOptions(entityOptionsBuilder);
        return this;
    }

    /// <summary>
    /// Configures options for a specific entity type within the admin panel using the configuration.
    /// </summary>
    /// <param name="entityConfiguration">The instance of the entity configuration class.</param>
    /// <typeparam name="TEntity">The type of entity for which options are being configured.</typeparam>
    public AdminOptionsBuilder ConfigureEntity<TEntity>(IEntityAdminConfiguration<TEntity> entityConfiguration)
        where TEntity : class
    {
        var entityOptionsBuilder = new EntityOptionsBuilder<TEntity>();
        entityConfiguration.Configure(entityOptionsBuilder);
        AddOrUpdateEntityOptions(entityOptionsBuilder);
        return this;
    }

    private void AddOrUpdateEntityOptions<TEntity>(EntityOptionsBuilder<TEntity> entityOptionsBuilder) where TEntity : class
    {
        var optionType = typeof(TEntity);
        var existingOption = options.EntityOptionsList.FirstOrDefault(o => o.EntityType == optionType);
        var newOption = entityOptionsBuilder.Create();
        if (existingOption != null)
        {
            UpdateProperties(existingOption, newOption);
            options.EntityOptionsList.Remove(existingOption);
        }
        options.EntityOptionsList.Add(newOption);
    }

    private static void UpdateProperties(EntityOptions source, EntityOptions destination)
    {
        var type = source.GetType();
        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            if (property is { CanRead: true, CanWrite: true })
            {
                var value = property.GetValue(source);
                if (value != null && string.IsNullOrEmpty(property.GetValue(destination).ToString()))
                {
                    property.SetValue(destination, value);
                }
            }
        }
    }

    /// <summary>
    /// Adds new groups to the configuration. Duplicates will be automatically filtered out based on group names.
    /// </summary>
    /// <param name="groups">The groups to be added.</param>
    public AdminOptionsBuilder AddGroups(IEnumerable<EntityGroup> groups)
    {
        foreach (var group in groups)
        {
            // Check for duplicated group's name.
            if (options.EntityGroupsList.Any(entityGroup => entityGroup.Name == group.Name))
            {
                continue;
            }

            options.EntityGroupsList.Add(group);
        }

        return this;
    }

    /// <summary>
    /// Configure the url to an external site.
    /// </summary>
    /// <param name="url">The url to be configured.</param>
    public AdminOptionsBuilder SetSiteUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentException($"Invalid URL: {url}");
        }

        if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            options.SiteUrl = url;
        }

        return this;
    }

    /// <summary>
    /// Disable the display of entity properties in Title Case format.
    /// </summary>
    public AdminOptionsBuilder DisableTitleCaseProperties()
    {
        options.TitleCaseProperties = false;
        return this;
    }

    /// <summary>
    /// Sets entity group headers to be expanded.
    /// </summary>
    public AdminOptionsBuilder SetGroupHeadersExpanded(bool groupHeadersExpanded)
    {
        options.GroupHeadersExpanded = groupHeadersExpanded;
        return this;
    }

    /// <summary>
    /// Sets static files folder location.
    /// </summary>
    public AdminOptionsBuilder SetStaticFilesFolder(string staticFilesFolder)
    {
        options.StaticFilesFolder = staticFilesFolder;
        return this;
    }

    /// <summary>
    /// Sets media files folder location.
    /// </summary>
    public AdminOptionsBuilder SetMediaFolder(string mediaFolder)
    {
        options.MediaFolder = mediaFolder;
        return this;
    }

    /// <summary>
    /// Sets max image size.
    /// </summary>
    /// <param name="maxImageSize">Max image size in megabytes.</param>
    public AdminOptionsBuilder SetMaxImageSize(int maxImageSize)
    {
        options.MaxImageSizeInMb = maxImageSize;
        return this;
    }

    /// <summary>
    /// Sets amount of max characters to all string properties, exceeded characters will be truncated.
    /// </summary>
    public AdminOptionsBuilder SetTruncationMaxCharacters(int truncationMaxCharacters)
    {
        options.TruncationMaxCharacters = truncationMaxCharacters;
        return this;
    }

    /// <summary>
    /// Disables global characters truncation.
    /// </summary>
    public AdminOptionsBuilder DisableCharactersTruncation()
    {
        options.TruncationMaxCharacters = default;
        return this;
    }

    /// <summary>
    /// Specifies whether to include all entities in the admin panel.
    /// This method can be used in conjunction with <see cref="IncludeEntities"/>
    /// or <see cref="NetForgeEntityAttribute"/> to include specific entities after excluding all.
    /// </summary>
    public AdminOptionsBuilder SetIncludeAllEntities(bool includeAllEntities)
    {
        options.IncludeAllEntities = includeAllEntities;
        return this;
    }

    /// <summary>
    /// Includes specific entities to the admin panel.
    /// </summary>
    /// <param name="tableTypes">The types of the entities to include.</param>
    public AdminOptionsBuilder IncludeEntities(params Type[] tableTypes)
    {
        options.IncludedEntities.AddRange(tableTypes);
        return this;
    }

    /// <summary>
    /// Sets the custom layout for the admin panel.
    /// </summary>
    /// <param name="layoutType">Type of the custom layout.</param>
    public AdminOptionsBuilder SetCustomLayout(Type layoutType)
    {
        options.CustomLayoutType = layoutType;
        return this;
    }

    /// <summary>
    /// Sets the custom head type for the admin panel.
    /// </summary>
    /// <param name="headType">The type of the custom head.</param>
    /// <returns>The current instance of <see cref="AdminOptionsBuilder"/>.</returns>
    public AdminOptionsBuilder SetCustomHeadType(Type headType)
    {
        options.CustomHeadType = headType;
        return this;
    }

    /// <summary>
    /// Configures callback options for entity lifecycle events.
    /// </summary>
    /// <param name="entityCallbackAction">An action to configure the entity callback options.</param>
    /// <returns>The current instance of <see cref="AdminOptionsBuilder"/>.</returns>
    public AdminOptionsBuilder ConfigureCallback(Action<EntityCallbackOptions> entityCallbackAction)
    {
        entityCallbackAction.Invoke(options.CallbackOptions);
        return this;
    }
}
