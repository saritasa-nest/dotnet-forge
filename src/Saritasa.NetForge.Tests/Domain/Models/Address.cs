using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Tests.Domain.Constants;

namespace Saritasa.NetForge.Tests.Domain.Models;

/// <summary>
/// Represents an address entity.
/// </summary>
internal class Address
{
    /// <summary>
    /// The unique identifier for the address.
    /// </summary>
    [NetForgeProperty(Order = 1)]
    public int Id { get; set; }

    /// <summary>
    /// The street name and number.
    /// </summary>
    [DisplayName(AddressConstants.StreetDisplayName)]
    [Description(AddressConstants.StreetDescription)]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// The city where the address is located.
    /// </summary>
    [Required]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// The postal code of the address.
    /// </summary>
    [NetForgeProperty(IsExcludedFromQuery = true)]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// The name of the country.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// The latitude coordinate of the address location.
    /// </summary>
    [NetForgeProperty(
        Order = 0,
        DisplayName = AddressConstants.LatitudeDisplayName,
        Description = AddressConstants.LatitudeDescription)]
    public double Latitude { get; set; }

    /// <summary>
    /// The longitude coordinate of the address location.
    /// </summary>
    [NetForgeProperty(IsHidden = true, IsHiddenFromListView = true, IsHiddenFromDetails = true)]
    public double Longitude { get; set; }

    /// <summary>
    /// The contact phone number associated with the address.
    /// </summary>
    public string ContactPhone { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Street} - {City}";
    }
}
