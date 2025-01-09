using Saritasa.NetForge.Blazor.Domain.Entities.Options;

namespace Saritasa.NetForge.Blazor.Domain;

/// <summary>
/// Builder class for configuring entity's calculated property options.
/// </summary>
public class CalculatedPropertyOptionsBuilder
{
    private readonly CalculatedPropertyOptions options = new();

    /// <summary>
    /// Creates calculated property options.
    /// </summary>
    /// <param name="propertyName">Calculated property name to create options for.</param>
    /// <returns>Calculated property options.</returns>
    public CalculatedPropertyOptions Create(string propertyName)
    {
        options.PropertyName = propertyName;

        return options;
    }

    /// <summary>
    /// Sets whether the calculated property should be hidden from the list view.
    /// </summary>
    /// <remarks>
    /// We do not have <c>SetIsHidden</c> and <c>SetIsHiddenFromDetails</c>
    /// because we do not display calculated properties on detail view.
    /// </remarks>
    public CalculatedPropertyOptionsBuilder SetIsHiddenFromListView(bool isHiddenFromListView)
    {
        options.IsHiddenFromListView = isHiddenFromListView;
        return this;
    }

    /// <summary>
    /// Sets new display name to calculated property.
    /// </summary>
    /// <param name="displayName">Name to display.</param>
    public CalculatedPropertyOptionsBuilder SetDisplayName(string displayName)
    {
        options.DisplayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets description to calculated property.
    /// </summary>
    /// <param name="description">Description.</param>
    public CalculatedPropertyOptionsBuilder SetDescription(string description)
    {
        options.Description = description;
        return this;
    }

    /// <summary>
    /// Sets order to calculated property.
    /// </summary>
    /// <param name="order">Order number.</param>
    public CalculatedPropertyOptionsBuilder SetOrder(int order)
    {
        options.Order = order;
        return this;
    }

    /// <summary>
    /// Set display format to calculated property value.
    /// </summary>
    /// <param name="displayFormat">Display format.</param>
    public CalculatedPropertyOptionsBuilder SetDisplayFormat(string displayFormat)
    {
        options.DisplayFormat = displayFormat;
        return this;
    }

    /// <summary>
    /// Set format provider to calculated property value.
    /// </summary>
    /// <param name="formatProvider">Format provider instance.</param>
    public CalculatedPropertyOptionsBuilder SetFormatProvider(IFormatProvider formatProvider)
    {
        options.FormatProvider = formatProvider;
        return this;
    }

    /// <summary>
    /// Sets the value to display when value of calculated property is empty.
    /// </summary>
    public CalculatedPropertyOptionsBuilder SetEmptyValueDisplay(string emptyValueDisplay)
    {
        options.EmptyValueDisplay = emptyValueDisplay;
        return this;
    }

    /// <summary>
    /// Sets the value to display the calculated property as HTML.
    /// </summary>
    public CalculatedPropertyOptionsBuilder SetDisplayAsHtml(bool displayAsHtml)
    {
        options.DisplayAsHtml = displayAsHtml;
        return this;
    }

    /// <summary>
    /// Sets amount of max characters. Exceeded characters will be truncated.
    /// </summary>
    public CalculatedPropertyOptionsBuilder SetTruncationMaxCharacters(int truncationMaxCharacters)
    {
        options.TruncationMaxCharacters = truncationMaxCharacters;
        return this;
    }
}
