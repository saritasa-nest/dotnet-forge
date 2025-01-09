using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Domain.Extensions;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Tests.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Helpers;
using Xunit;

namespace Saritasa.NetForge.Tests.EfCoreDataServiceTests;

/// <summary>
/// Tests for <see cref="IOrmDataService.UpdateAsync"/>.
/// </summary>
public class UpdateEntityTests : IDisposable
{
    private readonly TestDbContext testDbContext;
    private readonly IOrmDataService efCoreDataService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public UpdateEntityTests()
    {
        testDbContext = EfCoreHelper.CreateTestDbContext();
        efCoreDataService = EfCoreHelper.CreateEfCoreDataService(testDbContext);

        var shops = Fakers.ShopFaker.Generate(2);
        testDbContext.Shops.AddRange(shops);
        testDbContext.SaveChanges();

        testDbContext.ChangeTracker.Clear();
    }

    private bool disposedValue;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Deletes the database after one test is complete,
    /// so it gives us the same state of the database for every test.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue)
        {
            return;
        }

        if (disposing)
        {
            testDbContext.Dispose();
        }

        disposedValue = true;
    }

    /// <summary>
    /// Update valid entity.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_WithoutNavigations_ShouldUpdate()
    {
        // Arrange
        var updatedShop = await testDbContext.Shops.AsNoTracking().FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        const string newName = "Test222";
        updatedShop.Name = newName;

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(testDbContext.Shops, shop => shop.Name.Equals(newName));
    }

    /// <summary>
    /// Update entity navigation reference to a new one.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_NewNavigationReference_ShouldCreateAndUpdate()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Address).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        var newAddress = Fakers.AddressFaker.Generate();
        updatedShop.Address = newAddress;

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(testDbContext.Addresses, address => address.Street.Equals(newAddress.Street));
        Assert.Contains(shops, shop => shop.Address!.Street.Equals(newAddress.Street));
    }

    /// <summary>
    /// Update entity navigation reference to an existing one.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_ExistingNavigationReference_ShouldUpdate()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Address).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        var addressToUpdate = await testDbContext.Addresses
            .AsNoTracking()
            .FirstAsync(address => !updatedShop.Address!.Equals(address));
        updatedShop.Address = addressToUpdate;

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(shops, shop => shop.Address!.Street.Equals(addressToUpdate.Street));
    }

    /// <summary>
    /// Update entity navigation reference to null value.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_NavigationReferenceToNull_Success()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Address).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        updatedShop.Address = null;

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(shops, shop => shop.Address is null);
    }

    /// <summary>
    /// Update entity with adding new element to navigation collection.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_AddNewElementToNavigationCollection_ShouldCreateAndAdd()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Products).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        var newProduct = Fakers.ProductFaker.Generate();
        updatedShop.Products.Add(newProduct);

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(testDbContext.Products, product => product.Id == newProduct.Id);
        Assert.Contains(newProduct, updatedShop.Products);
    }

    /// <summary>
    /// Update entity with adding existing element to navigation collection.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_AddExistingElementToNavigationCollection_ShouldAdd()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Products).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        var productToAdd = await testDbContext.Products
            .AsNoTracking()
            .FirstAsync(product => !updatedShop.Products.Contains(product));
        updatedShop.Products.Add(productToAdd);

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.Contains(productToAdd, updatedShop.Products);
    }

    /// <summary>
    /// Update entity with removing element from navigation collection.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_RemoveElementFromNavigationCollection_ShouldRemove()
    {
        // Arrange
        var shops = testDbContext.Shops.Include(shop => shop.Products).AsNoTracking();

        var updatedShop = await shops.FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        var productToRemove = updatedShop.Products.First();
        updatedShop.Products.Remove(productToRemove);

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction: null, CancellationToken.None);

        // Assert
        Assert.DoesNotContain(productToRemove, updatedShop.Products);
    }

    /// <summary>
    /// Tests that after update action was called.
    /// </summary>
    [Fact]
    public async Task UpdateEntity_AfterUpdateAction_ShouldUpdate()
    {
        // Arrange
        var updatedShop = await testDbContext.Shops.AsNoTracking().FirstAsync();
        var originalShop = updatedShop.CloneJson()!;

        const string newName = "Test222";
        Action<IServiceProvider?, object, object> afterUpdateAction = (_, _, shop) =>
        {
            ((Shop)shop).Name = newName;
            testDbContext.SaveChanges();
        };

        // Act
        await efCoreDataService.UpdateAsync(updatedShop, originalShop, afterUpdateAction, CancellationToken.None);

        // Assert
        Assert.Contains(testDbContext.Shops, shop => shop.Name.Equals(newName));
    }
}
