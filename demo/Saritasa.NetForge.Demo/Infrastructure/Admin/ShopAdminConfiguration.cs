using Saritasa.NetForge.Demo.Infrastructure.UploadFiles.Strategies;
using Saritasa.NetForge.Demo.Models;
using Saritasa.NetForge.Domain.Enums;
using Saritasa.NetForge.DomainServices;
using Saritasa.NetForge.DomainServices.Interfaces;
using Saritasa.NetForge.Infrastructure.Abstractions.Interfaces;

namespace Saritasa.NetForge.Demo.Infrastructure.Admin;

/// <summary>
/// <see cref="Shop"/> admin panel configuration.
/// </summary>
public class ShopAdminConfiguration : IEntityAdminConfiguration<Shop>
{
    private readonly ServiceProvider serviceProvider;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ShopAdminConfiguration(IServiceCollection services)
    {
        serviceProvider = services.BuildServiceProvider();
    }

    /// <inheritdoc />
    public void Configure(EntityOptionsBuilder<Shop> entityOptionsBuilder)
    {
        entityOptionsBuilder
            .SetDescription("The base Shop entity.")
            .ConfigureSearch((serviceProvider, query, searchTerm) =>
            {
                return query.Where(e => e.Name.Contains(searchTerm));
            });

        entityOptionsBuilder.ConfigureProperty(shop => shop.IsOpen, builder =>
        {
            builder.SetIsSortable(true);
        });

        entityOptionsBuilder.ConfigureProperty(shop => shop.OpenedDate, builder =>
        {
            builder
                .SetIsSortable(true)
                .SetSearchType(SearchType.ContainsCaseInsensitive);
        });

        entityOptionsBuilder
            .IncludeNavigation<Address>(shop => shop.Address, navigationOptionsBuilder =>
            {
                navigationOptionsBuilder
                    .IncludeProperty(address => address.Street, builder =>
                    {
                        builder
                            .SetOrder(4)
                            .SetDisplayName("Address Street")
                            .SetDescription("Address street name.")
                            .SetEmptyValueDisplay("N/A")
                            .SetIsSortable(true)
                            .SetSearchType(SearchType.ContainsCaseInsensitive)
                            .SetShowNavigationDetails(isReadonly: false);
                    })
                    .IncludeProperty(address => address.City, builder =>
                    {
                        builder
                            .SetOrder(5)
                            .SetShowNavigationDetails(isReadonly: true);
                    })
                    .IncludeCalculatedProperty(address => address.FullAddress, builder =>
                    {
                        builder
                            .SetDisplayName("Entire Address")
                            .SetOrder(6);
                    });
            })
            .IncludeNavigation<Product>(shop => shop.Products, navigationOptionsBuilder =>
            {
                navigationOptionsBuilder.IncludeProperty(product => product.Id);
            })
            .IncludeNavigation<Supplier>(shop => shop.Suppliers, navigationOptionsBuilder =>
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

        entityOptionsBuilder.ConfigureProperty(shop => shop.Logo, builder =>
        {
            var s3Storage = serviceProvider.GetRequiredService<IBlobStorageService>();
            var cloudStorage = serviceProvider.GetRequiredService<ICloudBlobStorageService>();
            builder
                .SetIsImage(true)
                .SetOrder(1)
                .SetUploadFileStrategy(new UploadFileToS3Strategy(s3Storage, cloudStorage));
        });

        entityOptionsBuilder.ConfigureProperty(shop => shop.BuildingPhoto, builder =>
        {
            builder
                .SetIsImage(true)
                .SetUploadFileStrategy(new UploadBase64FileStrategy());
        });

        entityOptionsBuilder.ConfigureProperty(shop => shop.Name, builder =>
        {
            builder.SetTruncationMaxCharacters(25);
        });

        entityOptionsBuilder.SetAfterUpdateAction((serviceProvider, _, modifiedEntity) =>
        {
            var dbContext = serviceProvider!.GetRequiredService<ShopDbContext>();

            var randomNumber = Random.Shared.Next(0, 100);

            if (modifiedEntity.Address != null)
            {
                modifiedEntity.Address.City = $"Berlin {randomNumber}";
            }

            if (modifiedEntity.Suppliers.Count != 0)
            {
                modifiedEntity.Suppliers[0].IsActive = !modifiedEntity.Suppliers[0].IsActive;
            }

            dbContext.SaveChanges();
        });

        entityOptionsBuilder.ConfigureProperty(shop => shop.Suppliers, builder =>
        {
            builder.SetIsHidden(true);
        });

        entityOptionsBuilder.ConfigureProperty(shop => shop.Id, builder => builder.SetIsHidden(true));
    }
}
