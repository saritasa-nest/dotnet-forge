using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Blazor.Domain.Entities.Options;
using Xunit;

namespace Saritasa.NetForge.Tests;

/// <summary>
/// <see cref="AdminOptions"/> tests.
/// </summary>
public class AdminOptionsTests
{
    /// <summary>
    /// Verify that entity group headers are expanded when configured.
    /// </summary>
    [Fact]
    public void AdminOptions_ConfigureGroupHeadersToBeExpanded_ShouldBeExpanded()
    {
        // Arrange
        var adminOptionsBuilder = new AdminOptionsBuilder();
        var adminOptions = adminOptionsBuilder.Create();

        // Act
        adminOptionsBuilder.SetGroupHeadersExpanded(true);

        // Assert
        Assert.True(adminOptions.GroupHeadersExpanded);
    }

    /// <summary>
    /// Verify that entity group headers are not expanded when configured.
    /// </summary>
    [Fact]
    public void AdminOptions_ConfigureGroupHeadersToBeNotExpanded_ShouldNotBeExpanded()
    {
        // Arrange
        var adminOptionsBuilder = new AdminOptionsBuilder();
        var adminOptions = adminOptionsBuilder.Create();

        // Act
        adminOptionsBuilder.SetGroupHeadersExpanded(false);

        // Assert
        Assert.False(adminOptions.GroupHeadersExpanded);
    }

    /// <summary>
    /// Verify that entity group headers are expanded by default.
    /// </summary>
    [Fact]
    public void AdminOptions_GroupHeadersAreNotConfigured_ShouldBeExpanded()
    {
        // Arrange & Act
        var adminOptionsBuilder = new AdminOptionsBuilder();
        var adminOptions = adminOptionsBuilder.Create();

        // Assert
        Assert.True(adminOptions.GroupHeadersExpanded);
    }
}
