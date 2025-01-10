using System.Linq.Expressions;
using Saritasa.NetForge.Domain.Attributes;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.DomainServices.Extensions;

namespace Saritasa.NetForge.DomainServices;

/// <summary>
/// Builder class for configuring entity options within the admin panel.
/// </summary>
/// <typeparam name="TEntity">The type of entity for which options are being configured.</typeparam>
public class EntityOptionsBuilder<TEntity> where TEntity : class
{
    private readonly EntityOptions options = new(typeof(TEntity));

    /// <summary>
    /// Sets the description for the entity being configured.
    /// </summary>
    /// <param name="description">The description to set for the entity.</param>
    public EntityOptionsBuilder<TEntity> SetDescription(string description)
    {
        options.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the name for the entity being configured.
    /// </summary>
    /// <param name="name">The name to set for the entity.</param>
    public EntityOptionsBuilder<TEntity> SetDisplayName(string name)
    {
        options.DisplayName = name;
        return this;
    }

    /// <summary>
    /// Sets the plural name for the entity being configured.
    /// </summary>
    /// <param name="pluralName">The plural name to set for the entity.</param>
    public EntityOptionsBuilder<TEntity> SetPluralName(string pluralName)
    {
        options.PluralName = pluralName;
        return this;
    }

    /// <summary>
    /// Sets whether the entity should be hidden from the view.
    /// </summary>
    public EntityOptionsBuilder<TEntity> SetIsHidden(bool isHidden)
    {
        options.IsHidden = isHidden;
        return this;
    }

    /// <summary>
    /// Sets the group for the entity being configured.
    /// </summary>
    public EntityOptionsBuilder<TEntity> SetGroup(string groupName)
    {
        options.GroupName = groupName;
        return this;
    }

    /// <summary>
    /// Configure custom search.
    /// </summary>
    /// <param name="searchFunction">Custom search function.</param>
    public EntityOptionsBuilder<TEntity> ConfigureSearch(
        Func<IServiceProvider?, IQueryable<TEntity>, string, IQueryable<TEntity>> searchFunction)
    {
        // We need to override provided Func to perform successful cast from IQueryable<TEntity> to IQueryable<object>
        options.SearchFunction = (serviceProvider, query, searchTerm) =>
                searchFunction.Invoke(serviceProvider, query.Cast<TEntity>(), searchTerm);
        return this;
    }

    /// <summary>
    /// Configure custom query.
    /// </summary>
    /// <param name="customQueryFunction">Custom query function.</param>
    public EntityOptionsBuilder<TEntity> ConfigureCustomQuery(
        Func<IServiceProvider?, IQueryable<TEntity>, IQueryable<TEntity>> customQueryFunction)
    {
        options.CustomQueryFunction = (serviceProvider, query) =>
            customQueryFunction.Invoke(serviceProvider, query.Cast<TEntity>());
        return this;
    }

    /// <summary>
    /// Creates and returns the configured entity options.
    /// </summary>
    public EntityOptions Create()
    {
        return options;
    }

    /// <summary>
    /// Configures options for specific entity's property.
    /// </summary>
    /// <param name="propertyExpression">
    /// Expression that represents property. For example: <c>entity => entity.Name</c>.
    /// </param>
    /// <param name="propertyOptionsBuilderAction">An action that builds property options.</param>
    public EntityOptionsBuilder<TEntity> ConfigureProperty(
        Expression<Func<TEntity, object?>> propertyExpression,
        Action<PropertyOptionsBuilder> propertyOptionsBuilderAction)
    {
        var propertyOptionsBuilder = new PropertyOptionsBuilder();
        propertyOptionsBuilderAction.Invoke(propertyOptionsBuilder);

        var propertyName = propertyExpression.GetMemberName();
        var propertyOptions = propertyOptionsBuilder.Create(propertyName);

        options.PropertyOptions.Add(propertyOptions);
        return this;
    }

    /// <summary>
    /// Adds calculated properties for the specified entity type.
    /// </summary>
    /// <param name="propertyExpressions">An array of lambda expressions representing calculated properties.</param>
    public EntityOptionsBuilder<TEntity> AddCalculatedProperties(
        params Expression<Func<TEntity, object?>>[] propertyExpressions)
    {
        var propertyNames = propertyExpressions.Select(expression => expression.GetMemberName());
        options.CalculatedPropertyNames.AddRange(propertyNames);
        return this;
    }

    /// <summary>
    /// Adds navigation property to the entity.
    /// </summary>
    /// <param name="navigationExpression">Lambda expression representing navigation to include.</param>
    /// <param name="navigationOptionsBuilderAction">An action that builds navigation options.</param>
    public EntityOptionsBuilder<TEntity> IncludeNavigation<TNavigation>(
        Expression<Func<TEntity, object?>> navigationExpression,
        Action<NavigationOptionsBuilder<TNavigation>> navigationOptionsBuilderAction)
    {
        var navigationOptionsBuilder = new NavigationOptionsBuilder<TNavigation>();
        navigationOptionsBuilderAction.Invoke(navigationOptionsBuilder);

        var includedPropertyName = navigationExpression.GetMemberName();
        var navigationOptions = navigationOptionsBuilder.Create(includedPropertyName);

        options.NavigationOptions.Add(navigationOptions);
        return this;
    }

    /// <summary>
    /// Sets action that will be called after entity update.
    /// </summary>
    /// <param name="action">Action to call.</param>
    public EntityOptionsBuilder<TEntity> SetAfterUpdateAction(Action<IServiceProvider?, TEntity, TEntity> action)
    {
        options.AfterUpdateAction = (serviceProvider, x, y) => action(serviceProvider, (TEntity)x, (TEntity)y);
        return this;
    }

    /// <summary>
    /// Sets possibility to add a new entity.
    /// </summary>
    /// <param name="canAdd">Whether an entity can be added.</param>
    public EntityOptionsBuilder<TEntity> SetCanAdd(bool canAdd)
    {
        options.CanAdd = canAdd;
        return this;
    }

    /// <summary>
    /// Sets possibility to edit an entity.
    /// </summary>
    /// <param name="canEdit">Whether an entity can be edited.</param>
    public EntityOptionsBuilder<TEntity> SetCanEdit(bool canEdit)
    {
        options.CanEdit = canEdit;
        return this;
    }

    /// <summary>
    /// Sets possibility to delete an entity.
    /// </summary>
    /// <param name="canDelete">Whether an entity can be deleted.</param>
    public EntityOptionsBuilder<TEntity> SetCanDelete(bool canDelete)
    {
        options.CanDelete = canDelete;
        return this;
    }

    /// <summary>
    /// Excludes all properties by default.
    /// This method can be used in conjunction with <see cref="IncludeProperties"/>
    /// or <see cref="NetForgePropertyAttribute"/> to include specific properties after excluding all.
    /// </summary>
    public EntityOptionsBuilder<TEntity> ExcludeAllProperties()
    {
        options.ExcludeAllProperties = true;
        return this;
    }

    /// <summary>
    /// Includes specific properties.
    /// </summary>
    /// <param name="propertyExpressions">The expressions representing the properties to include.</param>
    public EntityOptionsBuilder<TEntity> IncludeProperties(params Expression<Func<TEntity, object?>>[] propertyExpressions)
    {
        var propertyNames = propertyExpressions.Select(expression => expression.GetMemberName());
        options.IncludedProperties.AddRange(propertyNames);
        return this;
    }

    /// <summary>
    /// Sets custom save message that displayed when the entity was saved successfully.
    /// </summary>
    /// <param name="entitySaveMessage">Entity save message.</param>
    /// <returns>The current instance of <see cref="AdminOptionsBuilder"/>.</returns>
    public EntityOptionsBuilder<TEntity> SetEntitySaveMessage(string entitySaveMessage)
    {
        options.EntitySaveMessage = entitySaveMessage;
        return this;
    }
}
