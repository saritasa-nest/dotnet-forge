using Microsoft.EntityFrameworkCore;
using Saritasa.NetForge.Blazor.Domain.Dtos;
using Saritasa.NetForge.Blazor.Domain.Enums;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Tests.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace Saritasa.NetForge.Tests.EfCoreDataServiceTests;

/// <summary>
/// Searching tests.
/// </summary>
[CollectionDefinition(Constants.DependencyInjection)]
public class SearchTests : TestBed<NetForgeFixture>
{
#pragma warning disable CA2213
    private readonly TestDbContext testDbContext;
#pragma warning restore CA2213
    private readonly IOrmDataService dataService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public SearchTests(ITestOutputHelper testOutputHelper, NetForgeFixture netForgeFixture)
        : base(testOutputHelper, netForgeFixture)
    {
        testDbContext = netForgeFixture.GetService<TestDbContext>(testOutputHelper)!;
        dataService = netForgeFixture.GetScopedService<IOrmDataService>(testOutputHelper)!;

        PopulateDatabaseWithTestData();
    }

    private void PopulateDatabaseWithTestData()
    {
        if (testDbContext.Addresses.Any())
        {
            return;
        }

        testDbContext.Addresses.Add(new Address
        {
            Street = "Main St.",
            City = "New York",
            Latitude = 100
        });
        testDbContext.Addresses.Add(new Address
        {
            Street = "Main Square St.",
            City = "London",
            Latitude = 101
        });
        testDbContext.Addresses.Add(new Address
        {
            Street = "Second Square St.",
            City = "London",
            Latitude = 102
        });
        testDbContext.Addresses.Add(new Address
        {
            Street = "Second main St.",
            City = "New York",
            Latitude = 10
        });
        testDbContext.Addresses.Add(new Address
        {
            Street = "Central",
            City = "London",
            Latitude = 222
        });
        testDbContext.Addresses.Add(new Address
        {
            Street = "central street",
            City = "New York",
            Latitude = 1
        });

        testDbContext.Products.Add(new Product
        {
            Supplier = new Supplier()
        });
        testDbContext.Products.Add(new Product
        {
            WeightInGrams = 111,
            Supplier = new Supplier
            {
                Name = "Supplier",
                City = "London"
            }
        });
        testDbContext.Products.Add(new Product
        {
            WeightInGrams = 222,
            Supplier = new Supplier
            {
                Name = "ABC",
                City = "New-York"
            }
        });
        testDbContext.SaveChanges();
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/>
    /// using <see cref="SearchType.ContainsCaseInsensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_ContainsCaseInsensitive_ShouldFind3()
    {
        // Arrange
        const string searchString = "ain";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.ContainsCaseInsensitive }
        };

        const int expectedCount = 3;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/>
    /// using <see cref="SearchType.StartsWithCaseSensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_StartsWithCaseSensitive_ShouldFind2()
    {
        // Arrange
        const string searchString = "Second";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.StartsWithCaseSensitive }
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/>
    /// using <see cref="SearchType.ExactMatchCaseInsensitive"/>.
    /// </summary>
    [Fact]
    public async Task Search_ExactMatchCaseInsensitive_ShouldFind1()
    {
        // Arrange
        const string searchString = "Central";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.ExactMatchCaseInsensitive }
        };

        const int expectedCount = 1;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/>
    /// using <see cref="SearchType.ExactMatchCaseInsensitive"/> to search values that contain <see langword="null"/>.
    /// </summary>
    [Fact]
    public async Task Search_ExactMatchCaseInsensitive_WithNoneSearchString_ShouldFind1()
    {
        // Arrange
        const string searchString = "None";
        var entityType = typeof(Product);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Product.WeightInGrams), SearchType = SearchType.ExactMatchCaseInsensitive }
        };

        const int expectedCount = 1;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Products, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/> when <see cref="SearchType.None"/>.
    /// </summary>
    [Fact]
    public async Task Search_WithoutSearch_ShouldFindAll()
    {
        // Arrange
        var searchString = "SearchString";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.None }
        };

        var expectedCount = await testDbContext.Addresses.CountAsync();

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/> when search string contains multiple words.
    /// </summary>
    [Fact]
    public async Task Search_WithMultipleWords_ShouldFind2()
    {
        // Arrange
        const string searchString = "sq lond";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.ContainsCaseInsensitive },
            new() { PropertyName = nameof(Address.City), SearchType = SearchType.ContainsCaseInsensitive },
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert

        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/> when search string contains quoted phrase.
    /// </summary>
    [Fact]
    public async Task Search_WithQuotedPhrase_ShouldFind2()
    {
        // Arrange
        const string searchString = "\"main St.\"";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Street), SearchType = SearchType.ContainsCaseInsensitive }
        };

        const int expectedCount = 2;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert
        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/> when searched property is not <see cref="string"/>.
    /// </summary>
    [Fact]
    public async Task Search_NotStringType_ShouldFind4()
    {
        // Arrange
        const string searchString = "10";
        var entityType = typeof(Address);
        var properties = new List<PropertySearchDto>
        {
            new() { PropertyName = nameof(Address.Latitude), SearchType = SearchType.ContainsCaseInsensitive }
        };

        const int expectedCount = 4;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Addresses, searchString, entityType, properties);

        // Assert
        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }

    /// <summary>
    /// Test for <seealso cref="IOrmDataService.Search"/> when searched property is not <see cref="string"/>.
    /// </summary>
    [Fact]
    public async Task Search_ByPropertyInsideNavigation_ShouldFind1()
    {
        // Arrange
        const string searchString = "London";
        var entityType = typeof(Product);
        var properties = new List<PropertySearchDto>
        {
            new()
            {
                PropertyName = nameof(Supplier.City),
                SearchType = SearchType.ContainsCaseInsensitive,
                NavigationName = nameof(Product.Supplier)
            }
        };

        const int expectedCount = 1;

        // Act
        var searchedData =
            dataService.Search(testDbContext.Products, searchString, entityType, properties);

        // Assert
        var actualCount = await searchedData.CountAsync();
        Assert.Equal(expectedCount, actualCount);
    }
}
