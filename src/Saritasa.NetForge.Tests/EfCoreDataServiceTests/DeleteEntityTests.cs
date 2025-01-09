using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Tests.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Fixtures;
using Saritasa.NetForge.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;
using Xunit.Microsoft.DependencyInjection.Attributes;

namespace Saritasa.NetForge.Tests.EfCoreDataServiceTests;

/// <summary>
/// Delete entity tests.
/// </summary>
[TestCaseOrderer(Constants.OrdererTypeName, Constants.OrdererAssemblyName)]
public class DeleteEntityTests : TestBed<NetForgeFixture>
{
#pragma warning disable CA2213
    private readonly TestDbContext testDbContext;
#pragma warning restore CA2213
    private readonly IOrmDataService efCoreDataService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public DeleteEntityTests(ITestOutputHelper testOutputHelper, NetForgeFixture netForgeFixture)
        : base(testOutputHelper, netForgeFixture)
    {
        testDbContext = _fixture.GetService<TestDbContext>(_testOutputHelper)!;
        efCoreDataService = _fixture.GetService<IOrmDataService>(_testOutputHelper)!;
    }

    /// <summary>
    /// Delete valid entity test.
    /// </summary>
    [Fact]
    [TestOrder(1)]
    public async Task DeleteEntity_ValidEntity_Success()
    {
        // Arrange
        var contactInfoType = typeof(ContactInfo);
        var contactInfo = Fakers.ContactInfoFaker.Generate();

        // Add the contactInfo to the database before attempting to delete it
        testDbContext.Add(contactInfo);

        // Act
        await efCoreDataService.DeleteAsync(contactInfo, contactInfoType, CancellationToken.None);

        // Assert
        Assert.DoesNotContain(testDbContext.ContactInfos, item => item.Id == contactInfo.Id);
    }

    /// <summary>
    /// Delete non-existing entity test.
    /// </summary>
    [Fact]
    [TestOrder(2)]
    public async Task DeleteEntity_NonExistingEntity_NoEffect()
    {
        // Arrange
        var contactInfoType = typeof(ContactInfo);
        var nonExistingContactInfo = Fakers.ContactInfoFaker.Generate();

        // Act
        // Assert that DbUpdateConcurrencyException is not thrown
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() =>
            efCoreDataService.DeleteAsync(nonExistingContactInfo, contactInfoType, CancellationToken.None));
    }

    /// <summary>
    /// Delete entity with invalid type test.
    /// </summary>
    [Fact]
    [TestOrder(2)]
    public async Task DeleteEntity_InvalidType_ThrowsException()
    {
        // Arrange
        var invalidType = typeof(int); // Assuming InvalidType is not a valid entity type

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            efCoreDataService.DeleteAsync(1, invalidType, CancellationToken.None));
    }
}
