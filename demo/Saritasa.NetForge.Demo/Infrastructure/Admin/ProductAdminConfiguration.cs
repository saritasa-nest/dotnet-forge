using Saritasa.NetForge.Blazor.Domain;
using Saritasa.NetForge.Blazor.Domain.Interfaces;
using Saritasa.NetForge.Blazor.Infrastructure.Abstractions.Interfaces;
using Saritasa.NetForge.Demo.Infrastructure.UploadFiles.Strategies;
using Saritasa.NetForge.Demo.Models;

namespace Saritasa.NetForge.Demo.Infrastructure.Admin;

/// <summary>
/// <see cref="Product"/> admin panel configuration.
/// </summary>
public class ProductAdminConfiguration : IEntityAdminConfiguration<Product>
{
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ProductAdminConfiguration(IServiceCollection services)
    {
        serviceProvider = services.BuildServiceProvider();
    }

    /// <inheritdoc />
    public void Configure(EntityOptionsBuilder<Product> entityOptionsBuilder)
    {
        entityOptionsBuilder.IncludeNavigation<Supplier>(product => product.Supplier, navigationOptionsBuilder =>
        {
            navigationOptionsBuilder
                .IncludeProperty(supplier => supplier.Name, propertyOptionsBuilder =>
                {
                    propertyOptionsBuilder.SetDisplayName("Supplier Name");
                })
                .IncludeProperty(supplier => supplier.City, propertyOptionsBuilder =>
                {
                    propertyOptionsBuilder.SetDisplayName("Supplier City");
                });
        });

        entityOptionsBuilder.IncludeNavigation<Shop>(product => product.Shop, navigationOptionsBuilder =>
        {
            navigationOptionsBuilder
                .IncludeProperty(shop => shop.Id, propertyOptionsBuilder =>
                {
                    propertyOptionsBuilder.SetDisplayName("Shop Id");
                })
                .IncludeProperty(shop => shop.Logo, propertyOptionsBuilder =>
                {
                    var s3Storage = serviceProvider.GetRequiredService<IBlobStorageService>();
                    var cloudStorage = serviceProvider.GetRequiredService<ICloudBlobStorageService>();
                    propertyOptionsBuilder
                        .SetIsImage(true)
                        .SetUploadFileStrategy(new UploadFileToS3Strategy(s3Storage, cloudStorage));
                })
                .IncludeProperty(shop => shop.BuildingPhoto, propertyOptionsBuilder =>
                {
                    propertyOptionsBuilder
                        .SetIsImage(true)
                        .SetUploadFileStrategy(new UploadBase64FileStrategy());
                });
        });

        entityOptionsBuilder.ConfigureProperty(product => product.UpdatedDate, builder =>
        {
            builder.SetIsReadOnly(true);
        });
    }
}