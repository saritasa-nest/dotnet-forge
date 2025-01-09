# NetForge - Admin Panel for ASP.NET Core 6 & 7 & 8

The **NetForge** is a library that provides a user-friendly and intuitive user interface for performing CRUD operations on your database entities within your .NET applications.

- [NetForge - Admin Panel for ASP.NET Core 6 \& 7 \& 8](#netforge---admin-panel-for-aspnet-core-6--7--8)
- [How to Use](#how-to-use)
- [Global Configurations](#global-configurations)
  - [Customizing the Endpoint](#customizing-the-endpoint)
  - [Customizing the Title](#customizing-the-title)
  - [Configuring Authorization](#configuring-authorization)
  - [Search](#search)
  - [View Site URL](#view-site-url)
  - [Grouping](#grouping)
  - [Customizing the UI](#customizing-the-ui)
    - [Main Layout Overriding](#main-layout-overriding)
    - [Head Tag Overriding](#head-tag-overriding)
    - [Create Groups for Entities](#create-groups-for-entities)
    - [Configuration](#configuration)
    - [Headers Expansion](#headers-expansion)
  - [Exclude All Entities and Include Specific Only](#exclude-all-entities-and-include-specific-only)
- [Customizing Entities](#customizing-entities)
  - [Fluent API](#fluent-api)
  - [Creating an Entity Configuration Class](#creating-an-entity-configuration-class)
  - [Data Attributes](#data-attributes)
  - [Custom Query](#custom-query)
  - [After Update Action](#after-update-action)
- [Customizing Entity Properties](#customizing-entity-properties)
  - [Fluent API](#fluent-api-1)
  - [Data Attributes](#data-attributes-1)
  - [Display Formatting](#display-formatting)
  - [Data Sorting](#data-sorting)
  - [Calculated Properties](#calculated-properties)
  - [Display Properties as Title Case](#display-properties-as-title-case)
  - [Default Values](#default-values)
  - [Navigation Properties](#navigation-properties)
  - [Exclude All Properties and Include Specific Only](#exclude-all-properties-and-include-specific-only)
  - [Exclude Property from Query](#exclude-property-from-query)
  - [Formatting Property as HTML](#formatting-property-as-html)
  - [Generated Properties](#generated-properties)
  - [Rich Text Field](#rich-text-field)
  - [Image Properties](#image-properties)
    - [Configuration](#configuration-1)
    - [Max image size](#max-image-size)
  - [Read-only Properties](#read-only-properties)
    - [Configuration](#configuration-2)
  - [String Truncate](#string-truncate)
    - [Configuration](#configuration-3)
  - [Multiline Text Field Property](#multiline-text-field-property)
    - [Configuration](#configuration-4)
  - [License](#license)

# How to Use

Add NetForge to your service collection in `Program.cs`:

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.UseEntityFramework(efOptionsBuilder =>
    {
        efOptionsBuilder.UseDbContext<MyDbContext>();
    });
    ...
});
```

Make your application to use the admin panel:

```csharp
app.UseNetForge();
```

# Global Configurations

## Customizing the Endpoint

By default, NetForge Admin is running on */admin* but you can configure to use your custom endpoint like this:

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.UseEndpoint("/manage");
    ...
});
```

## Customizing the Title

You can customize the header title like this:

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetHeaderTitle("title");
    ...
});
```

And customize the HTML title like this:

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetHtmlTitle("title");
    ...
});
```

## Configuring Authorization

You can customize the access policy by requiring specific [Identity roles](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles):

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.AddAccessRoles("Role1", "Role2", "Role3");
    ...
});
```

Alternatively, you can use a custom function to perform authorization checks. Example:

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.ConfigureAuth(serviceProvider =>
    {
        // Allow all authenticated users to see the Admin Panel.
        var httpContext = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        return Task.FromResult(httpContext?.User.Identity?.IsAuthenticated ?? false);
    });
});
```

## Search

You can read about search [here](docs/SEARCH.md).

## View Site URL

Located in the top right corner of the admin panel is a "View Site" link, configurable to direct users to the website URL.
The default URL is "/". You can customize this value using the Fluent API:

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.ConfigureUrl("https://www.example.com/");
});
```

## Grouping

Group rows of entities into categories and make it easier for users to navigate and understand the data presented.

## Customizing the UI

### Main Layout Overriding

You can override the default layout of the admin panel.
To do this, create a new layout in your host project and specify its type in the configuration.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetCustomLayout(typeof(CustomLayout));
});
```

Your custom layout should inherit from the `AdminBaseLayout` class.

The example of the custom component with navigation bar and footer:
```csharp
@using MudBlazor
@inherits Saritasa.NetForge.Blazor.Shared.AdminBaseLayout

<MudThemeProvider />
<MudDialogProvider />
<MudSnackbarProvider />

<MudAppBar Color="Color.Primary" Elevation="4">
    <MudText Typo="Typo.h6">My Application</MudText>
    <MudSpacer />
    <MudNavMenu>
        <MudNavLink Href="/about">About</MudNavLink>
        <MudNavLink Href="/contact">Contact</MudNavLink>
    </MudNavMenu>
</MudAppBar>

@Body

<footer>
    <MudPaper Class="pa-4" Elevation="4">
        <MudText Typo="Typo.body2">My Application</MudText>
        <MudSpacer />
        <MudNavMenu>
            <MudNavLink Href="/privacy">Privacy Policy</MudNavLink>
            <MudNavLink Href="/terms">Terms of Service</MudNavLink>
        </MudNavMenu>
    </MudPaper>
</footer>
```

### Head Tag Overriding

You can inject custom meta tags or page title into the head tag of the admin panel.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetCustomHeadType(typeof(CustomHead));
});
```

Example of injecting custom meta tags or a page title into the head tag of the admin panel.
```csharp
﻿@using Microsoft.AspNetCore.Components.Web

<PageTitle>This is the custom page title.</PageTitle>
<meta name="custom-meta" content="content-meta">
```

**Note:** If you are using a `Custom Layout`, you must add the dynamic component into the custom layout:

```csharp
@using Saritasa.NetForge.Domain.Entities.Options
@inject AdminOptions AdminOptions;

<HeadContent>
    @if (AdminOptions.CustomHeadType is not null)
    {
        <DynamicComponent Type="AdminOptions.CustomHeadType" />
    }
</HeadContent>
```

Example:

```csharp
@using MudBlazor
@using Saritasa.NetForge.Domain.Entities.Options
@using Microsoft.AspNetCore.Components.Web
@inherits Saritasa.NetForge.Blazor.Shared.AdminBaseLayout
@inject AdminOptions AdminOptions;

<HeadContent>
    @if (AdminOptions.CustomHeadType is not null)
    {
        <DynamicComponent Type="AdminOptions.CustomHeadType" />
    }
</HeadContent>

<MudThemeProvider />
<MudDialogProvider />
...
...
```

### Create Groups for Entities

Before assigning entities to specific groups, users need to define the groups to which the entities will belong.
To create a new group, utilize the Fluent API through `AdminOptionsBuilder`. A name is required for each group, and a description is optional.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.UseEntityFramework(efOptionsBuilder =>
    {
        efOptionsBuilder.UseDbContext<ShopDbContext>();
    }).AddGroups(new List<EntityGroup>
    {
        new EntityGroup{ Name = "Product", Description = "Contains all information related to products" },
        new EntityGroup{ Name = "Shop"}
    }).ConfigureEntity<Shop>(entityOptionsBuilder =>
    {
        entityOptionsBuilder.SetDescription("The base Shop entity.");
    });
});
```

### Configuration

By default, entities are assigned to the "empty" group. Grouping can be customized either through the Fluent API or by using attributes.
When assigning entities to a group, users only need to specify the group's name. If user specifies a group that does not exists for an entity,
that entity will belong to the default group.

**Fluent API:**

By utilizing `EntityOptionsBuilder`, user can set group for entity using group's name.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.UseEntityFramework(efOptionsBuilder =>
    {
        efOptionsBuilder.UseDbContext<ShopDbContext>();
    }).AddGroups(new List<EntityGroup>
    {
        new EntityGroup{ Name = "Product", Description = "Contains all information related to products" },
        new EntityGroup{ Name = "Shop"}
    }).ConfigureEntity<Shop>(entityOptionsBuilder =>
    {
        entityOptionsBuilder.SetGroup("Shop");
        entityOptionsBuilder.SetDescription("The base Shop entity.");
    });
});
```

**Data Attribute:**

```csharp
[NetForgeEntity(GroupName = "Product")]
public class ProductTag
```

### Headers Expansion

You can customize expanded header of all groups. By default, all groups are expanded.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetGroupHeadersExpanded(true);
});
```

## Exclude All Entities and Include Specific Only

You can exclude all entities and include only specific ones.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetIncludeAllEntities(false);
    optionsBuilder.IncludeEntities(typeof(Shop), typeof(Product));
});
```

Or you can include specific entities using the data attribute:
```csharp
[NetForgeEntity]
public class Shop
```

# Customizing Entities

In the admin panel, you can customize the way entities are displayed using the Fluent API or special attributes. This enables you to set various properties for your entities, such as their name, description, plural name, etc.

## Fluent API

```csharp
appBuilder.Services.AddNetForge(optionsBuilder =>
{
     optionsBuilder.UseEntityFramework(efOptionsBuilder =>
     {
         efOptionsBuilder.UseDbContext<MyDbContext>();
     });

     // Set the description to display for the Shop entity.
     optionsBuilder.ConfigureEntity<Shop>(entityOptionsBuilder =>
     {
         entityOptionsBuilder.SetDescription("Represents the shop.");
     });

     // Hide ProductTag from the admin panel.
     optionsBuilder.ConfigureEntity<ProductTag>(entityOptionsBuilder =>
     {
         entityOptionsBuilder.SetIsHidden(true);
     });
});
```

## Creating an Entity Configuration Class

To reduce the amount of the code all configuration for an entity type can also be extracted to a separate class.

To create an entity configuration for a specific entity type, create a new class that implements the `IEntityAdminConfiguration<TEntity>` interface, where `TEntity` is the type of the entity you want to configure. For example, if you want to configure the `Product` entity, your class might look like this:

```csharp
public class ProductAdminConfiguration : IEntityAdminConfiguration<Product>
{
    public void Configure(EntityOptionsBuilder<Product> builder)
    {
        // Define entity-specific settings here.
    }
}

// Add this to your Program.cs.
appBuilder.Services.AddNetForge(optionsBuilder =>
{
     optionsBuilder.ConfigureEntity(new ProductAdminConfiguration());

     // Other settings...
});
```

## Data Attributes

You can also customize your entities by applying special attributes directly to your entity classes.

**NetForgeEntityAttribute**

```csharp
[NetForgeEntity(DisplayName = "Entity", PluralName = "Entities", Description = "This is an entity description.")]
public class Entity
{
    // Entity properties...
}
```

You can use the built-in `System.ComponentModel.DescriptionAttribute` and `System.ComponentModel.DisplayNameAttribute` to specify descriptions and display names for your entity classes.

For example:

```csharp
[Description("Custom entity description.")]
[DisplayName("Custom Entity Display Name")]
public class AnotherEntity
{
    // Entity properties...
}
```

## Custom Query

You can configure your query for specific entity.

```csharp
.ConfigureEntity<Shop>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureCustomQuery((serviceProvider, query) =>
    {
        return query.Where(e => e.IsOpen == true);
    });
})
```

## After Update Action

You can configure action that will be performed after entity update.

```csharp
.ConfigureEntity<Address>(entityOptionsBuilder =>
{
    entityOptionsBuilder.SetAfterUpdateAction((serviceProvider, originalEntity, modifiedEntity) =>
    {
        var dbContext = serviceProvider!.GetRequiredService<ShopDbContext>();

        const string country = "Germany";
        if (originalEntity.Country == country)
        {
            return;
        }

        if (modifiedEntity.Country == country)
        {
            modifiedEntity.PostalCode = "99998";
        }

        dbContext.SaveChanges();
    });
})
```

You can use `ServiceProvider` to access your services.

# Customizing Entity Properties

You can customize entity properties as well. For example, you can change display name, add description, hide it or change property column order.

## Fluent API

```csharp
optionsBuilder.ConfigureEntity<Address>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureProperty(address => address.Id, propertyBuilder =>
    {
        propertyBuilder
            .SetDescription("Item identifier.")
            .SetOrder(2);
    });

    entityOptionsBuilder.ConfigureProperty(address => address.ContactPhone, propertyBuilder =>
    {
        propertyBuilder
            .SetDisplayName("Phone")
            .SetDescription("Address contact phone.")
            .SetOrder(1);
    });

    entityOptionsBuilder.ConfigureProperty(address => address.PostalCode, propertyBuilder =>
    {
        propertyBuilder.SetIsHidden(true);
    });

    entityOptionsBuilder.ConfigureProperty(address => address.City, propertyBuilder =>
    {
        propertyBuilder.SetDisplayName("Town");
    });
});
```

## Data Attributes

Properties also customizable via attributes.

**NetForgeEntityAttribute**

```csharp
[NetForgeProperty(DisplayName = "Custom property display name", Description = "Custom property description.", Order = 5)]
public string Property { get; set; }
```
**Built-in `Description` and `DisplayName` Attributes**

```csharp
[Description("Custom property description.")]
[DisplayName("Custom property display name")]
public string Property { get; set; }
```

## Display Formatting

You can configure the display format for the properties values. See [string.Format](https://learn.microsoft.com/en-us/dotnet/standard/base-types/composite-formatting#format-string-component).

**Using Data Attributes**

You can apply the `[NetForgeProperty]` attribute to an entity property and specify the display format:

```csharp
[NetForgeProperty(DisplayFormat = "{0:C}")]
public decimal Price { get; set; }
```

In this example, the Price property will be displayed using the currency format.

**Using Fluent API**

Alternatively, you can use the Fluent API to configure the display format and format provider for an entity property:

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.ConfigureEntity<Product>(entityOptionsBuilder =>
    {
        entityOptionsBuilder.ConfigureProperty(product => product.Price, propertyBuilder =>
        {
            propertyBuilder.SetDisplayFormat("{0:C}");

            // Use Euro as a currency.
            propertyBuilder.SetFormatProvider(CultureInfo.GetCultureInfo("fr-FR"));
        });
    });

    // Other settings...
});
```
## Data Sorting

You can apply alphabet sorting to some properties. By default, they are not sortable.

It is configurable via `[NetForgeProperty]` and `Fluent API`.

**Using Data Attribute**

```csharp
[NetForgeProperty(IsSortable = true)]
public string Name { get; set; }
```

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(shop => shop.OpenedDate, builder =>
{
    builder.SetIsSortable(true);
});
```

You can sort multiple properties at once. It can be achieved by pressing sort buttons with `CTRL`.

Sorting can be cancelled by pressing on it with `ALT`.

## Calculated Properties

Calculated properties are properties that don't have a direct representation in your database but are computed based on other existing properties. These properties can be useful for displaying calculated values in the admin panel.

They behave like an ordinary property, but have less functionality.

```csharp
entityOptionsBuilder.ConfigureCalculatedProperty(address => address.FullAddress, propertyBuilder =>
{
    propertyBuilder.SetDisplayName("Full Address");
});
```

## Display Properties as Title Case

By default, all entity properties are displayed in Title Case.

For example, the `Product` entity will have the property `StockQuantity`. By default, it will be displayed as `Stock Quantity` in the admin panel.
This behavior can be disabled, and the entities will use CamelCase display instead.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.DisableTitleCaseProperties();
});
```

## Default Values

Users can customize the value used for displaying the empty record values. By default, it will be displayed as "-" (a dash).

**Using Fluent API**

```csharp
optionsBuilder.ConfigureEntity<User>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureProperty(user => user.DateOfBirth,
        propertyBuilder => propertyBuilder.SetEmptyValueDisplay("N/A"));
});
```

**Using Attribute**

```csharp
[NetForgeProperty(EmptyValueDisplay = "N/A")]
public string Property { get; set; }
```

## Navigation Properties

You can read about navigation properties [here](docs/NAVIGATIONS.md).

## Exclude All Properties and Include Specific Only

You can exclude all properties and include only specific ones. It works with both ordinary and navigation properties.

**Using Fluent API**

```csharp
optionsBuilder.ConfigureEntity<User>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ExcludeAllProperties();
    entityOptionsBuilder.IncludeProperties(user => user.Id, user => user.Name);
});
```

Or you can include specific properties using the **Attribute**:
```csharp
[NetForgeProperty]
public string Property { get; set; }
```

## Exclude Property from Query

You can explicitly control whether a property should be excluded from the data query.

**Using Fluent API**

```csharp
optionsBuilder.ConfigureEntity<User>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureProperty(user => user.DateOfBirth,
        propertyBuilder => propertyBuilder.SetIsExcludedFromQuery(true));
});
```

**Using Data Attribute**

```csharp
[NetForgeProperty(IsExcludeFromQuery = true)]
public string Property { get; set; }
```

## Formatting Property as HTML

You can configure certain entity properties to be rendered as HTML content in the data grid. This feature ensures that HTML tags within these properties are not escaped, allowing for richer and more dynamic data presentation.

**Using Fluent API**

```csharp
optionsBuilder.ConfigureEntity<User>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureProperty(user => user.Id,
        propertyBuilder => propertyBuilder.SetDisplayAsHtml(true));
});
```

**Using Data Attribute**

```csharp
[NetForgeProperty(DisplayAsHtml = true)]
public string Property { get; set; }
```

## Generated Properties

Generated properties will not be displayed on the create or edit entity pages. Entity framework example of generated property:

```csharp
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public DateTime CreatedAt { get; set; }
```

## Rich Text Field

RTF provides some common text formatting options like paragraphs, links, tables, etc.
The [ClassicEditor of CKEditor 5](https://ckeditor.com/docs/ckeditor5/latest/examples/builds/classic-editor.html) is used by the admin panel.

The configuration is the following:

**Using Data Attribute**
```csharp
[NetForgeProperty(IsRichTextField = true)]
public required string Description { get; set; }
```

**Using Fluent API**
```csharp
optionsBuilder.ConfigureEntity<Product>(entityOptionsBuilder =>
{
    entityOptionsBuilder.ConfigureProperty(product => product.Description,
        propertyBuilder => propertyBuilder.SetIsRichTextField(true));
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
```

## Image Properties

You can add properties that will be displayed as images.

### Configuration

**Using Fluent API**

You can create your own implementation of `IUploadFileStrategy` interface and pass it to `SetUploadFileStrategy` configuration method.

This interface has methods `UploadFileAsync` that is calling when file is uploaded and `GetFileSource` that is calling when file should be displayed.

We have some examples of strategies [here](demo/Saritasa.NetForge.Demo/Infrastructure/UploadFiles/Strategies).

```csharp
entityOptionsBuilder.ConfigureProperty(shop => shop.BuildingPhoto, builder =>
{
    builder
        .SetIsImage(true)
        .SetUploadFileStrategy(new UploadBase64FileStrategy());
});
```

### Max image size

You can set max image size in the application. Default value for max image size is 10 MB.

``` csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetMaxImageSize(15);
});
```

## Read-only Properties

You can mark a property as read only. Such property cannot be changed on create and edit pages.

### Configuration

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(product => product.UpdatedDate, builder =>
{
    builder.SetIsReadOnly(true);
});
```

**Using Data Attribute**

```csharp
[NetForgeProperty(IsReadOnly = true)]
public string Property { get; set; }
```

## String Truncate

You can set the max characters amount for string properties.

### Configuration

**Global**

You can set max characters for the all strings. Default value is 50.

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.SetTruncationMaxCharacters(60);
});
```

You can disable this behavior in this way:

```csharp
services.AddNetForge(optionsBuilder =>
{
    optionsBuilder.DisableCharactersTruncation();
});
```

**For the Property**

You can set max characters to each property individually.

**Using Data Attribute**

```csharp
[NetForgeProperty(TruncationMaxCharacters = 20)]
public string Name { get; set; }
```

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(shop => shop.Name, builder =>
{
    builder.SetTruncationMaxCharacters(25);
});
```

## Multiline Text Field Property

You can mark a property as multiline text field. It allows input of text in several text rows. Disabled by default.

### Configuration

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(address => address.Street, builder =>
{
    builder.SetIsMultiline();
});
```

**Using Data Attribute**

```csharp
[MultilineText]
public required string Street { get; set; }
```

Also, there are some auxiliary properties that are used in the multiple text field.

**Number of lines**

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(address => address.Street, builder =>
{
    builder.SetIsMultiline(lines: 15); // sets the lines value as 15
});
```

**Using Data Attribute**

```csharp
[MultilineText(Lines = 15)]
public required string Street { get; set; }
```

**Max Number of Lines**

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(address => address.Street, builder =>
{
    builder.SetIsMultiline(maxLines: 15); // sets the max lines value as 15
});
```

**Using Data Attribute**

```csharp
[MultilineText(MaxLines = 15)]
public required string Street { get; set; }
```

**Auto Grow**

`IsAutoGrow` property identifies whether the height of the text field automatically changes with the number of lines of text.

**Using Fluent API**

```csharp
entityOptionsBuilder.ConfigureProperty(address => address.Street, builder =>
{
    builder.SetIsMultiline(autoGrow: true);
});
```

**Using Data Attribute**

```csharp
[MultilineText(IsAutoGrow = true)]
public required string Street { get; set; }
```

Contributors
------------

* Saritasa http://www.saritasa.com

License
-------

The project is licensed under the terms of the BSD license. Refer to LICENSE.txt for more information.
