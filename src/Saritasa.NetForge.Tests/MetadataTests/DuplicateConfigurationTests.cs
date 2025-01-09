using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Xunit;

namespace Saritasa.NetForge.Tests.MetadataTests;

/// <summary>
/// Duplicate entity options configuration tests.
/// </summary>
public class DuplicateEntityOptionsConfigurationTests
{
    /// <summary>
    /// Verify that configure entity with both fluent API and configuration class only create 1 instance of the entity options.
    /// </summary>
    [Fact]
    public void ConfigureEntity_WithBothFluentApiAndConfigurationClass_ShouldNotCreateDuplicateOptions()
    {
        // Arrange
        var adminOptionsBuilder = new AdminOptionsBuilder();
        const int expectedOptionsCount = 1;
        var adminOptions = adminOptionsBuilder.Create();

        // Act
        adminOptionsBuilder.ConfigureEntity(new UserAdminConfiguration())
        .ConfigureEntity<User>(builder =>
        {
            builder.SetIsHidden(true);
        });

        // Assert
        var entityOptionsCount = adminOptions.EntityOptionsList.Count;
        Assert.Equal(expectedOptionsCount, entityOptionsCount);
    }

    /// <summary>
    /// Verify that configure entity with both fluent API and configuration class will take the second configuration values.
    /// </summary>
    [Fact]
    public void ConfigureEntity_WithBothFluentApiAndConfigurationClass_ShouldCreateOverrideValue()
    {
        // Arrange
        var adminOptionsBuilder = new AdminOptionsBuilder();
        const string mockDescription = "This is an override description";
        var adminOptions = adminOptionsBuilder.Create();

        // Act
        adminOptionsBuilder.ConfigureEntity(new UserAdminConfiguration())
        .ConfigureEntity<User>(builder =>
        {
            builder.SetDescription(mockDescription);
        });

        // Assert
        var userDescription = adminOptions.EntityOptionsList.FirstOrDefault(x => x.EntityType == typeof(User))
            ?.Description;
        Assert.Equal(mockDescription, userDescription);
    }
}
