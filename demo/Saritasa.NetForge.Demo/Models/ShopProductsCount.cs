using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Domain.Attributes;
using Saritasa.NetForge.Demo.Constants;

namespace Saritasa.NetForge.Demo.Models;

/// <summary>
/// Count of products in the shop.
/// </summary>
[Keyless]
[NetForgeEntity(GroupName = GroupConstants.Shops, PluralName = "Shop Products Count")]
public class ShopProductsCount
{
    /// <summary>
    /// Shop.
    /// </summary>
    public required Shop Shop { get; init; }

    /// <summary>
    /// Products count.
    /// </summary>
    public int ProductsCount { get; init; }
}
