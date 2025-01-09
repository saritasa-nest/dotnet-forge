using System.ComponentModel;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Demo.Constants;

namespace Saritasa.NetForge.Demo.Models;

/// <summary>
/// Represents a product entity.
/// </summary>
[NetForgeEntity(Description = "The product in the shop.", GroupName = GroupConstants.Shops)]
public class Product
{
    /// <summary>
    /// The unique identifier for the product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the product.
    /// </summary>
    [NetForgeProperty(TruncationMaxCharacters = 20, IsSortable = true)]
    public required string Name { get; set; }

    /// <summary>
    /// The description of the product.
    /// </summary>
    [MultilineText(Lines = 5, MaxLines = 10, IsAutoGrow = true)]
    [NetForgeProperty(IsRichTextField = true, Description = "The description of the Product.")]
    public required string Description { get; set; }

    /// <summary>
    /// The price of the product.
    /// </summary>
    [NetForgeProperty(DisplayFormat = "{0:C}")]
    public decimal Price { get; set; }

    /// <summary>
    /// The highest price on the product by all the time.
    /// </summary>
    [NetForgeProperty(Description = "Maximum price that the product had.")]
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// The quantity of the product in stock.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// The average number of product purchases.
    /// </summary>
    public int? AveragePurchaseCount { get; set; }

    /// <summary>
    /// The weight of the product in grams.
    /// </summary>
    public float? WeightInGrams { get; set; }

    /// <summary>
    /// The length of the product in centimeters.
    /// </summary>
    public float LengthInCentimeters { get; set; }

    /// <summary>
    /// The width of the product in centimeters.
    /// </summary>
    public float WidthInCentimeters { get; set; }

    /// <summary>
    /// The height of the product in centimeters.
    /// </summary>
    public float HeightInCentimeters { get; set; }

    /// <summary>
    /// The volume of the product.
    /// </summary>
    public double Volume { get; set; }

    /// <summary>
    /// The barcode of the product.
    /// </summary>
    public long Barcode { get; set; }

    /// <summary>
    /// Whether the product still available.
    /// </summary>
    [NetForgeProperty(Description = "The product still in sale.")]
    public bool IsAvailable { get; set; }

    /// <summary>
    /// Whether sales of the product was ended.
    /// </summary>
    public bool? IsSalesEnded { get; set; }

    /// <summary>
    /// The date and time when the product was created.
    /// </summary>
    [NetForgeProperty(IsReadOnly = true)]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// The date and time when the product was last updated.
    /// </summary>
    public DateTime UpdatedDate { get; set; }

    /// <summary>
    /// The date when the product was removed.
    /// </summary>
    public DateTime? RemovedAt { get; set; }

    /// <summary>
    /// The date when sales of the product was ended.
    /// </summary>
    public DateTimeOffset? EndOfSalesDate { get; set; }

    /// <summary>
    /// The previous supply date of the product.
    /// </summary>
    public DateOnly PreviousSupplyDate { get; set; }

    /// <summary>
    /// The next date when the product will be supplied.
    /// </summary>
    public DateOnly? NextSupplyDate { get; set; }

    /// <summary>
    /// The list of tags associated with the product.
    /// </summary>
    public List<ProductTag> Tags { get; set; } = new();

    /// <summary>
    /// Supplier that providing this product.
    /// </summary>
    public required Supplier Supplier { get; set; }

    /// <summary>
    /// The category of the product.
    /// </summary>
    [Description("Product category.")]
    public Category Category { get; set; }

    /// <summary>
    /// The shop where this product can be bought.
    /// </summary>
    public Shop? Shop { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Id} - {Name}";
    }
}
