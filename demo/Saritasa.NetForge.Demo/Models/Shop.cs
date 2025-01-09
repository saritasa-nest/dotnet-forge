using System.ComponentModel;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Blazor.Domain.Enums;
using Saritasa.NetForge.Demo.Constants;

namespace Saritasa.NetForge.Demo.Models;

/// <summary>
/// Represents a shop entity.
/// </summary>
[NetForgeEntity(GroupName = GroupConstants.Shops)]
public class Shop
{
    /// <summary>
    /// Unique identifier for the shop.
    /// </summary>
    [Description("Shop identifier.")]
    [DisplayName("Identifier")]
    public int Id { get; set; }

    /// <summary>
    /// The name of the shop.
    /// </summary>
    [Description("Shop name.")]
    [NetForgeProperty(IsSortable = true)]
    public required string Name { get; set; }

    /// <summary>
    /// The address of the shop.
    /// </summary>
    [NetForgeProperty(Description = "Represents address of the shop.")]
    public Address? Address { get; set; }

    /// <summary>
    /// The date when the shop was opened.
    /// </summary>
    public DateTime OpenedDate { get; set; }

    /// <summary>
    /// The open time of the shop.
    /// </summary>
    public TimeOnly? OpenTime { get; set; }

    /// <summary>
    /// The close time of the shop.
    /// </summary>
    public TimeOnly? CloseTime { get; set; }

    /// <summary>
    /// The total sales amount for the shop.
    /// </summary>
    [NetForgeProperty(
        DisplayName = "Sales",
        Description = "Total sales.",
        IsSortable = true,
        SearchType = SearchType.StartsWithCaseSensitive)]
    public decimal TotalSales { get; set; }

    /// <summary>
    /// The shop is currently open for business.
    /// </summary>
    public bool IsOpen { get; set; }

    /// <summary>
    /// The list of products available in the shop.
    /// </summary>
    [DisplayName("ShopProducts")]
    public List<Product> Products { get; set; } = new();

    /// <summary>
    /// The shop owner's contact information.
    /// </summary>
    public ContactInfo? OwnerContact { get; set; }

    /// <summary>
    /// The list of suppliers.
    /// </summary>
    public List<Supplier> Suppliers { get; set; } = new();

    /// <summary>
    /// Count of suppliers.
    /// </summary>
    public int SupplierCount => Suppliers.Count;

    /// <summary>
    /// Path to image file.
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Bytes that represents image.
    /// </summary>
    [NetForgeProperty(Order = 2, Description = "Photo of the shop from the outside.")]
    public string? BuildingPhoto { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Id}; {Name}";
    }
}
