using Saritasa.NetForge.Blazor.Controls.CustomFields;
using Saritasa.NetForge.Blazor.Controls.CustomFields.DateTimeFields;
using Saritasa.NetForge.Blazor.Domain.UseCases.Metadata.GetEntityById;

namespace Saritasa.NetForge.Blazor.Infrastructure.Helpers;

/// <summary>
/// Helps to get custom field for property type.
/// </summary>
public static class CustomFieldHelper
{
    private static readonly IEnumerable<Type> NumberFieldTypes = new List<Type>
    {
        typeof(short), typeof(short?),
        typeof(ushort), typeof(ushort?),
        typeof(int), typeof(int?),
        typeof(uint), typeof(uint?),
        typeof(long), typeof(long?),
        typeof(ulong), typeof(ulong?),
        typeof(float), typeof(float?),
        typeof(double), typeof(double?),
        typeof(decimal), typeof(decimal?)
    };

    private static readonly IEnumerable<Type> DateOnlyFieldTypes = new List<Type>
    {
        typeof(DateOnly), typeof(DateOnly?)
    };

    private static readonly IEnumerable<Type> DateTimeFieldTypes = new List<Type>
    {
        typeof(DateTime), typeof(DateTime?),
        typeof(DateTimeOffset), typeof(DateTimeOffset?)
    };

    private static readonly IEnumerable<Type> TimeOnlyFieldTypes = new List<Type>
    {
        typeof(TimeSpan), typeof(TimeSpan?),
        typeof(TimeOnly), typeof(TimeOnly?)
    };

    private static readonly IEnumerable<Type> BooleanFieldTypes = new List<Type>
    {
        typeof(bool), typeof(bool?)
    };

    private static readonly IEnumerable<Type> TextFieldTypes = new List<Type>
    {
        typeof(string)
    };

    private static IReadOnlyDictionary<IEnumerable<Type>, Type> TypeMappingDictionary { get; } =
        new Dictionary<IEnumerable<Type>, Type>
        {
            { NumberFieldTypes, typeof(NumberField<>) },
            { DateTimeFieldTypes, typeof(DateTimeField) },
            { DateOnlyFieldTypes, typeof(DateOnlyField) },
            { TimeOnlyFieldTypes, typeof(TimeOnlyField) },
            { BooleanFieldTypes, typeof(BoolField) },
            { TextFieldTypes, typeof(TextField) }
        };

    /// <summary>
    /// Gets custom field <see cref="Type"/> depending on <paramref name="property"/>.
    /// </summary>
    public static Type GetComponentType(PropertyMetadataDto property)
    {
        if (property.IsImage)
        {
            return typeof(UploadImage);
        }

        if (property.IsRichTextField)
        {
            return typeof(CkEditorField);
        }

        if (property is NavigationMetadataDto navigation)
        {
            return navigation.IsCollection
                ? typeof(NavigationCollectionField<>).MakeGenericType(property.ClrType!.GetGenericArguments().First())
                : typeof(NavigationField);
        }

        foreach (var (types, inputType) in TypeMappingDictionary)
        {
            if (types.Contains(property.ClrType!))
            {
                return inputType.IsGenericType ? inputType.MakeGenericType(property.ClrType!) : inputType;
            }
        }

        // Text field is a default one.
        return property.ClrType!.IsEnum ? typeof(EnumField) : typeof(TextField);
    }
}
