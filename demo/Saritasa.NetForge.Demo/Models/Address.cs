using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Blazor.Domain.Enums;
using Saritasa.NetForge.Demo.Constants;

namespace Saritasa.NetForge.Demo.Models;

/// <summary>
/// Represents an address entity.
/// </summary>
[Comment("Represents the address of the shop.")]
[NetForgeEntity(GroupName = GroupConstants.Shops)]
public class Address
{
    /// <summary>
    /// The unique identifier for the address.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The street name and number.
    /// </summary>
    [NetForgeProperty(DisplayName = "Street name", Description = "Street name without street number.",
        Order = 3, SearchType = SearchType.StartsWithCaseSensitive)]
    [Required]
    public required string Street { get; set; }

    /// <summary>
    /// The city where the address is located.
    /// </summary>
    [NetForgeProperty(Order = 4, SearchType = SearchType.StartsWithCaseSensitive)]
    [Required]
    public required string City { get; set; }

    /// <summary>
    /// Display name of the address.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The postal code of the address.
    /// </summary>
    [NetForgeProperty(Order = 6)]
    [Required]
    public required string PostalCode { get; set; }

    /// <summary>
    /// The name of the country.
    /// </summary>
    [NetForgeProperty(Description = "Country name.", Order = 5)]
    [Required]
    public required string Country { get; set; }

    /// <summary>
    /// The latitude coordinate of the address location.
    /// </summary>
    [NetForgeProperty(Order = 0)]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the address location.
    /// </summary>
    [NetForgeProperty(IsHiddenFromDetails = true)]
    public double Longitude { get; set; }

    /// <summary>
    /// The contact phone number associated with the address.
    /// </summary>
    [Required]
    public required string ContactPhone { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return DisplayName;
    }
}
