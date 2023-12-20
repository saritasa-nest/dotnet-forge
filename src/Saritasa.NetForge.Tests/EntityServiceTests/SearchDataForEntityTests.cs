﻿using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Domain.Enums;
using Saritasa.NetForge.Infrastructure.EfCore.Services;
using Saritasa.NetForge.Tests.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Helpers;
using Saritasa.NetForge.UseCases.Common;
using Saritasa.NetForge.UseCases.Services;
using Xunit;

namespace Saritasa.NetForge.Tests.EntityServiceTests;

/// <summary>
/// Create entity tests.
/// </summary>
public class SearchDataForEntityTests : IDisposable
{
    private TestDbContext TestDbContext { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SearchDataForEntityTests()
    {
        var dbOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("NetForgeTest")
            .Options;

        TestDbContext = new TestDbContext(dbOptions);
        TestDbContext.Database.EnsureCreated();

        TestDbContext.Addresses.Add(new Address
        {
            Id = 1,
            Street = "Main St.",
            City = "New York"
        });
        TestDbContext.Addresses.Add(new Address
        {
            Id = 2,
            Street = "Main Square St.",
            City = "London"
        });
        TestDbContext.Addresses.Add(new Address
        {
            Id = 3,
            Street = "Second Square St.",
            City = "London"
        });
        TestDbContext.Addresses.Add(new Address
        {
            Id = 4,
            Street = "Second main St.",
            City = "New York"
        });
        TestDbContext.Addresses.Add(new Address
        {
            Id = 5,
            Street = "Central",
            City = "London"
        });
        TestDbContext.Addresses.Add(new Address
        {
            Id = 6,
            Street = "central street",
            City = "New York"
        });
        TestDbContext.SaveChanges();
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
                TestDbContext.Database.EnsureDeleted();
                TestDbContext.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/>
    /// using <see cref="SearchType.ContainsCaseInsensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_ContainsCaseInsensitive_ShouldFind3()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "ain";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.ContainsCaseInsensitive)
        };

        const int expectedCount = 3;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/>
    /// using <see cref="SearchType.StartsWithCaseSensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_StartsWithCaseSensitive_ShouldFind2()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "Second";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.StartsWithCaseSensitive)
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/>
    /// using <see cref="SearchType.ExactMatchCaseInsensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_ExactMatchCaseInsensitive_ShouldFind1()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "Central";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.ExactMatchCaseInsensitive)
        };

        const int expectedCount = 1;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/>
    /// using <see cref="SearchType.ExactMatchCaseInsensitive"/> to search values that contain <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task Search_ExactMatchCaseInsensitiveWhenSearchStringIsNone_ShouldFind2()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "None";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.ExactMatchCaseInsensitive)
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/>
    /// using <see cref="SearchType.ExactMatchCaseInsensitive"/> to search values that contain <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task Search_WithoutSearch_ShouldFindAll()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "None";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.None)
        };

        var expectedCount = await TestDbContext.Addresses.CountAsync();

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/> when search string contains multiple words.
    /// </summary>
    [Fact]
    public async Task Search_WithMultipleWords_ShouldFind2()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "sq lond";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.ContainsCaseInsensitive),
            (nameof(Address.City), SearchType.ContainsCaseInsensitive)
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="EfCoreDataService.Search"/> when search string contains quoted phrase.
    /// </summary>
    [Fact]
    public async Task Search_WithQuotedPhrase_ShouldFind2()
    {
        // Arrange
        var efCoreDataService = EfCoreHelper.CreateEfCoreDataService(TestDbContext);

        const string searchString = "\"main St.\"";
        var entityType = typeof(Address);
        var propertiesWithSearchTypes = new List<(string, SearchType)>
        {
            (nameof(Address.Street), SearchType.ContainsCaseInsensitive)
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            efCoreDataService.Search(TestDbContext.Addresses, searchString, entityType, propertiesWithSearchTypes);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }
}
