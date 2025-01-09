using System.ComponentModel.DataAnnotations;
using Moq;
using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Blazor.Domain.UseCases.Interfaces;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.Services;
using Saritasa.NetForge.Blazor.Domain.UseCases.Services;
using Saritasa.NetForge.Tests.Domain.Models;
using Saritasa.NetForge.Tests.Helpers;
using Xunit;

namespace Saritasa.NetForge.Tests.EntityServiceTests;

/// <summary>
/// Tests for <see cref="EntityService.ValidateEntity"/>.
/// </summary>
public class ValidateEntityTests
{
    private readonly IEntityService entityService;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ValidateEntityTests()
    {
        var testDbContext = EfCoreHelper.CreateTestDbContext();
        var adminOptionsBuilder = new AdminOptionsBuilder();
        var adminMetadataService = new AdminMetadataService(
            EfCoreHelper.CreateEfCoreMetadataService(testDbContext),
            adminOptionsBuilder.Create(),
            MemoryCacheHelper.CreateMemoryCache());

        entityService = new EntityService(adminMetadataService,
            EfCoreHelper.CreateEfCoreDataService(testDbContext),
            new Mock<IServiceProvider>().Object);
    }

    /// <summary>
    /// Test for case when single validation attribute on single property is valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_SinglePropertySingleValidate_ShouldNotHaveError()
    {
        // Prepare
        var instance = new Address { City = "City" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.True(result, "The validate result must be true");
        Assert.Empty(errors);
    }

    /// <summary>
    /// Test for case when single validation attribute on single property is not valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_SinglePropertySingleValidate_ShouldHaveError()
    {
        // Prepare
        const string property = "City";
        var instance = new Address();

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        // The result should be false because the City property is required.
        Assert.False(result, "The validate result must be false");
        Assert.Single(errors);

        // The error should contain the property.
        var propertiesHaveError = errors.SelectMany(e => e.MemberNames).Distinct().ToList();
        Assert.Contains(property, propertiesHaveError);
    }

    /// <summary>
    /// Test for case when multiple validation attribute on single property is valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_SinglePropertyMultipleValidate_ShouldNotHaveError()
    {
        // Prepare
        var instance = new ContactInfo { FullName = "FullName", PhoneNumber = "0123-456-789" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.True(result, "The validate result must be true");
        Assert.Empty(errors);
    }

    /// <summary>
    /// Test for case when multiple validation attribute on single property is not valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_SinglePropertyMultipleValidate_ShouldHaveError()
    {
        // Prepare
        const string property = "Description";
        var instance = new Product();

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.False(result, "The validate result must be false");
        Assert.Single(errors);

        // The error should contain the property.
        var propertiesHaveError = errors.SelectMany(e => e.MemberNames).Distinct().ToList();
        Assert.Contains(property, propertiesHaveError);
    }

    /// <summary>
    /// Test for case when multiple validation attribute on multiple property is valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_MultiplePropertyMultipleValidate_ShouldNotHaveError()
    {
        // Prepare
        var instance = new ProductTag { Name = "Name", Description = "Description" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.True(result);
        Assert.Empty(errors);
    }

    /// <summary>
    /// Test for case when multiple validation attribute on multiple property is not valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_MultiplePropertyMultipleValidate_ShouldHaveError()
    {
        // Prepare
        const string firstProperty = "Name";
        const string secondProperty = "Description";
        var instance = new ProductTag { Name = "", Description = "" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.False(result, "The validate result must false.");

        // The error should contain the property.
        var memberHaveError = errors.SelectMany(e => e.MemberNames).Distinct().ToList();
        Assert.Contains(firstProperty, memberHaveError);
        Assert.Contains(secondProperty, memberHaveError);
    }

    /// <summary>
    /// Test for case when custom validation attribute is valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_CustomValidationAttribute_ShouldNotHaveError()
    {
        // Prepare
        var instance = new ContactInfo { FullName = "FullName", PhoneNumber = "0123-456-789" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.True(result, "The validate result must be true");
        Assert.Empty(errors);
    }

    /// <summary>
    /// Test for case when custom validation attribute is not valid.
    /// </summary>
    [Fact]
    public void ValidateEntity_CustomValidationAttribute_ShouldHaveError()
    {
        // Prepare
        var property = "PhoneNumber";
        var instance = new ContactInfo { FullName = "FullName", PhoneNumber = "123123123" };

        var errors = new List<ValidationResult>();

        // Act
        var result = entityService.ValidateEntity(instance, [], ref errors);

        // Assert
        Assert.False(result, "The validate result must be false");
        Assert.Single(errors);

        // The error should contain the property.
        var memberHaveError = errors.SelectMany(e => e.MemberNames).Distinct().ToList();
        Assert.Contains(property, memberHaveError);
    }
}
