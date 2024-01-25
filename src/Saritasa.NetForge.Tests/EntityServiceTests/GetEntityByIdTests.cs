﻿using System.ComponentModel;
using Moq;
using Saritasa.NetForge.Domain.Attributes;
using Saritasa.NetForge.DomainServices;
using Saritasa.NetForge.Tests.Constants;
using Saritasa.NetForge.Tests.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Helpers;
using Saritasa.NetForge.Tests.Utilities;
using Saritasa.NetForge.UseCases.Interfaces;
using Saritasa.NetForge.UseCases.Metadata.Services;
using Saritasa.NetForge.UseCases.Services;
using Saritasa.Tools.Domain.Exceptions;
using Xunit;

namespace Saritasa.NetForge.Tests.EntityServiceTests;

/// <summary>
/// Tests for <see cref="EntityService.GetEntityByIdAsync"/>.
/// </summary>
public class GetEntityByIdTests : IDisposable
{
    private const string AttributeTestEntityId = "Addresses";
    private const string FluentApiTestEntityId = "Shops";

    private readonly TestDbContext testDbContext;
    private readonly IEntityService entityService;
    private readonly AdminOptionsBuilder adminOptionsBuilder;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GetEntityByIdTests()
    {
        testDbContext = EfCoreHelper.CreateTestDbContext();
        adminOptionsBuilder = new AdminOptionsBuilder();
        var adminMetadataService = new AdminMetadataService(
            EfCoreHelper.CreateEfCoreMetadataService(testDbContext),
            adminOptionsBuilder.Create(),
            MemoryCacheHelper.CreateMemoryCache());

        entityService = new EntityService(
            AutomapperHelper.CreateAutomapper(),
            adminMetadataService,
            EfCoreHelper.CreateEfCoreDataService(testDbContext),
            new Mock<IServiceProvider>().Object);
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
        if (!disposedValue)
        {
            if (disposing)
            {
                testDbContext.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Test for case when string id is valid.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_ValidStringId_ShouldBeNotNull()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.NotNull(entity);
    }

    /// <summary>
    /// Test for case when string id is invalid.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_InvalidStringId_ShouldThrowNotFoundException()
    {
        // Arrange
        const string invalidStringId = "Addresses2";

        // Act
        var getEntityByIdCall = () => entityService.GetEntityByIdAsync(invalidStringId, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(getEntityByIdCall);
    }

    /// <summary>
    /// Test for case when navigation included to entity.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithNavigations_ShouldBeNotNull()
    {
        // Arrange
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder.IncludeNavigations(entity => entity.Address);
        });

        const string navigationPropertyName = nameof(Shop.Address);

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(entity.Properties, property => property.Name.Equals(navigationPropertyName));
    }

    /// <summary>
    /// Test for case when property excluded from query via Fluent API.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithExcludedFromQueryPropertyViaFluentApi_ShouldNotContainExcludedProperty()
    {
        // Arrange
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder
                .ConfigureProperty(shop => shop.IsOpen, optionsBuilder => optionsBuilder.SetIsExcludedFromQuery(true));
        });

        const string excludedPropertyName = nameof(Shop.IsOpen);

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.DoesNotContain(entity.Properties, property => property.Name.Equals(excludedPropertyName));
    }

    /// <summary>
    /// Test for case when property excluded from query via <see cref="NetForgePropertyAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithExcludedFromQueryPropertyViaAttribute_ShouldNotContainExcludedProperty()
    {
        // Arrange
        const string excludedPropertyName = nameof(Address.PostalCode);

        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.DoesNotContain(entity.Properties, property => property.Name.Equals(excludedPropertyName));
    }

    /// <summary>
    /// Test for case when property is hidden via Fluent API.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithHiddenPropertyViaFluentApi_PropertyShouldBeHidden()
    {
        // Arrange
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder.ConfigureProperty(shop => shop.IsOpen, optionsBuilder => optionsBuilder.SetIsHidden(true));
        });

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(entity.Properties, property => property.IsHidden);
    }

    /// <summary>
    /// Test for case when property is hidden via <see cref="NetForgePropertyAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithHiddenPropertyViaAttribute_PropertyShouldBeHidden()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(entity.Properties, property => property.IsHidden);
    }

    /// <summary>
    /// Test for case when properties don't have ordering.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithoutOrdering_PrimaryKeyShouldBeFirst()
    {
        // Arrange
        const string expectedPropertyName = nameof(Shop.Id);

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPropertyName, entity.Properties.First().Name);
    }

    /// <summary>
    /// Test for case when properties have ordering via Fluent API.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithOrderingViaFluentApi_OrderedPropertyShouldBeFirst()
    {
        // Arrange
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder
                .ConfigureProperty(shop => shop.TotalSales, optionsBuilder => optionsBuilder.SetOrder(0))
                .ConfigureProperty(shop => shop.Id, optionsBuilder => optionsBuilder.SetOrder(1));
        });
        const string expectedPropertyName = nameof(Shop.TotalSales);

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPropertyName, entity.Properties.First().Name);
    }

    /// <summary>
    /// Test for case when properties have ordering via <see cref="NetForgePropertyAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithOrderingViaAttribute_OrderedPropertyShouldBeFirst()
    {
        // Arrange
        const string expectedPropertyName = nameof(Address.Latitude);

        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPropertyName, entity.Properties.First().Name);
    }

    /// <summary>
    /// Test for case when property has set display name via Fluent API.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDisplayNameViaFluentApi_DisplayNameShouldChange()
    {
        // Arrange
        const string displayName = "Sales";
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder.ConfigureProperty(shop => shop.TotalSales,
                    optionsBuilder => optionsBuilder.SetDisplayName(displayName));
        });

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.DisplayName.Equals(displayName));
    }

    /// <summary>
    /// Test for case when property has set display name via <see cref="NetForgePropertyAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDisplayNameViaAttribute_DisplayNameShouldChange()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.DisplayName.Equals(AddressConstants.LatitudeDisplayName));
    }

    /// <summary>
    /// Test for case when property has set display name via <see cref="DisplayNameAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDisplayNameViaBuiltInAttribute_DisplayNameShouldChange()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.DisplayName.Equals(AddressConstants.StreetDisplayName));
    }

    /// <summary>
    /// Test for case when property has set description via Fluent API.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDescriptionViaFluentApi_DescriptionShouldChange()
    {
        // Arrange
        const string description = "Information about total sales that was made by shop.";
        adminOptionsBuilder.ConfigureEntity<Shop>(builder =>
        {
            builder.ConfigureProperty(
                shop => shop.TotalSales,
                optionsBuilder => optionsBuilder.SetDescription(description));
        });

        // Act
        var entity = await entityService.GetEntityByIdAsync(FluentApiTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.Description.Equals(description));
    }

    /// <summary>
    /// Test for case when property has set description via <see cref="NetForgePropertyAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDescriptionViaAttribute_DescriptionShouldChange()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.Description.Equals(AddressConstants.LatitudeDescription));
    }

    /// <summary>
    /// Test for case when property has set description via <see cref="DescriptionAttribute"/>.
    /// </summary>
    [Fact]
    public async Task GetEntityByIdAsync_WithPropertyDescriptionViaBuiltInAttribute_DescriptionShouldChange()
    {
        // Act
        var entity = await entityService.GetEntityByIdAsync(AttributeTestEntityId, CancellationToken.None);

        // Assert
        Assert.Contains(
            entity.Properties, property => property.Description.Equals(AddressConstants.StreetDescription));
    }
}