using Saritasa.NetForge.Domain;
using Saritasa.NetForge.Domain.Entities.Options;
using Saritasa.NetForge.Domain.Enums;

namespace Saritasa.NetForge.DomainServices;

/// <summary>
/// Builder class for configuring entity's property options.
/// </summary>
public class PropertyOptionsBuilder
{
    private readonly PropertyOptions options = new();

    /// <summary>
    /// Creates property options.
    /// </summary>
    /// <param name="propertyName">Property name to create options for.</param>
    /// <returns>Property options.</returns>
    public PropertyOptions Create(string propertyName)
    {
        options.PropertyName = propertyName;

        return options;
    }

    /// <summary>
    /// Sets whether the property should be hidden from the view.
    /// </summary>
    public PropertyOptionsBuilder SetIsHidden(bool isHidden)
    {
        options.IsHidden = isHidden;
        return this;
    }

    /// <summary>
    /// Sets whether the property should be hidden from the list view.
    /// </summary>
    public PropertyOptionsBuilder SetIsHiddenFromListView(bool isHiddenFromListView)
    {
        options.IsHiddenFromListView = isHiddenFromListView;
        return this;
    }

    /// <summary>
    /// Sets whether the property should be hidden from the list view.
    /// </summary>
    public PropertyOptionsBuilder SetIsHiddenFromDetails(bool isHiddenFromDetails)
    {
        options.IsHiddenFromDetails = isHiddenFromDetails;
        return this;
    }

    /// <summary>
    /// Sets whether the property should be excluded from the query.
    /// </summary>
    public PropertyOptionsBuilder SetIsExcludedFromQuery(bool isExcludedFromQuery)
    {
        options.IsExcludedFromQuery = isExcludedFromQuery;
        return this;
    }

    /// <summary>
    /// Sets new display name to property.
    /// </summary>
    /// <param name="displayName">Name to display.</param>
    public PropertyOptionsBuilder SetDisplayName(string displayName)
    {
        options.DisplayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets description to property.
    /// </summary>
    /// <param name="description">Description.</param>
    public PropertyOptionsBuilder SetDescription(string description)
    {
        options.Description = description;
        return this;
    }

    /// <summary>
    /// Sets order to property.
    /// </summary>
    /// <param name="order">Order number.</param>
    public PropertyOptionsBuilder SetOrder(int order)
    {
        options.Order = order;
        return this;
    }

    /// <summary>
    /// Set display format to property value.
    /// </summary>
    /// <param name="displayFormat">Display format.</param>
    public PropertyOptionsBuilder SetDisplayFormat(string displayFormat)
    {
        options.DisplayFormat = displayFormat;
        return this;
    }

    /// <summary>
    /// Set format provider to property value.
    /// </summary>
    /// <param name="formatProvider">Format provider instance.</param>
    public PropertyOptionsBuilder SetFormatProvider(IFormatProvider formatProvider)
    {
        options.FormatProvider = formatProvider;
        return this;
    }

    /// <summary>
    /// Sets search type to property.
    /// </summary>
    public PropertyOptionsBuilder SetSearchType(SearchType searchType)
    {
        options.SearchType = searchType;
        return this;
    }

    /// <summary>
    /// Sets whether the property is sortable.
    /// </summary>
    public PropertyOptionsBuilder SetIsSortable(bool isSortable)
    {
        options.IsSortable = isSortable;
        return this;
    }

    /// <summary>
    /// Sets the value to display when value of property is empty.
    /// </summary>
    public PropertyOptionsBuilder SetEmptyValueDisplay(string emptyValueDisplay)
    {
        options.EmptyValueDisplay = emptyValueDisplay;
        return this;
    }

    /// <summary>
    /// Sets the value to display the property as HTML.
    /// </summary>
    public PropertyOptionsBuilder SetDisplayAsHtml(bool displayAsHtml)
    {
        options.DisplayAsHtml = displayAsHtml;
        return this;
    }

    /// <summary>
    /// Sets whether the field is a rich text field.
    /// </summary>
    public PropertyOptionsBuilder SetIsRichTextField(bool isRichTextField)
    {
        options.IsRichTextField = isRichTextField;
        return this;
    }

    /// <summary>
    /// Marks this property as image.
    /// </summary>
    public PropertyOptionsBuilder SetIsImage(bool isImage)
    {
        options.IsImage = isImage;
        return this;
    }

    /// <summary>
    /// Marks this property as read only.
    /// </summary>
    public PropertyOptionsBuilder SetIsReadOnly(bool isReadOnly)
    {
        options.IsReadOnly = isReadOnly;
        return this;
    }

    /// <summary>
    /// Sets amount of max characters. Exceeded characters will be truncated.
    /// </summary>
    public PropertyOptionsBuilder SetTruncationMaxCharacters(int truncationMaxCharacters)
    {
        options.TruncationMaxCharacters = truncationMaxCharacters;
        return this;
    }

    /// <summary>
    /// Sets upload file strategy.
    /// </summary>
    public PropertyOptionsBuilder SetUploadFileStrategy(IUploadFileStrategy uploadFileStrategy)
    {
        options.UploadFileStrategy = uploadFileStrategy;
        return this;
    }

    /// <summary>
    /// Marks this property as multiline and sets properties that are used with the multiple text field.
    /// Such as auto grow, lines and max lines.
    /// </summary>
    /// <param name="autoGrow">
    /// Auto grow flag that identifies whether the height of the text field automatically changes with the number of lines of text.
    /// </param>
    /// <param name="lines">Amount of lines.</param>
    /// <param name="maxLines">Amount of max lines.</param>
    public PropertyOptionsBuilder SetIsMultiline(bool autoGrow = false, int lines = 5, int maxLines = 5)
    {
        options.IsMultiline = true;

        options.IsAutoGrow = autoGrow;
        options.MaxLines = maxLines;
        options.Lines = lines;

        return this;
    }

    /// <summary>
    /// Enables feature to show a corresponding navigation entity details on List View page
    /// when this property displayed as part of some navigation.
    /// </summary>
    /// <param name="isReadonly">
    /// When true a navigation entity details will be displayed as modal window,
    /// otherwise you will be navigated to edit page.
    /// </param>
    public PropertyOptionsBuilder SetShowNavigationDetails(bool isReadonly)
    {
        if (isReadonly)
        {
            options.CanDisplayDetails = true;
        }
        else
        {
            options.CanBeNavigatedToDetails = true;
        }
        return this;
    }
}
