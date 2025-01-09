using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Tests.Domain.Models;
using Xunit;

namespace Saritasa.NetForge.Tests.MetadataTests;

/// <summary>
/// HTML format for entity properties tests.
/// </summary>
public class PropertyFormatHtmlTests
{
    /// <summary>
    /// Configure entity property through fluent API test.
    /// </summary>
    [Fact]
    public void ConfigurePropertyWithCustomHtml_ThroughFluentApi_Success()
    {
        // Arrange
        var adminOptionsBuilder = new AdminOptionsBuilder();
        var adminOptions = adminOptionsBuilder.Create();

        // Act
        adminOptionsBuilder.ConfigureEntity<User>(builder =>
        {
            builder.ConfigureProperty(user => user.FirstName, propBuilder => propBuilder.SetDisplayAsHtml(true));
        });

        // Assert
        var userEntity = adminOptions.EntityOptionsList.FirstOrDefault(x => x.EntityType == typeof(User));
        var userFirstNameAllowedHtml = userEntity?.PropertyOptions.FirstOrDefault(x => x.PropertyName == "FirstName")?.DisplayAsHtml;

        Assert.True(userFirstNameAllowedHtml);
    }
}
