﻿using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Saritasa.NetForge.Blazor.Controls;

/// <summary>
/// Represents C# type mapped to HTML field.
/// </summary>
public partial class CustomField
{
    /// <summary>
    /// Property name.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public string PropertyName { get; set; } = null!;

    /// <summary>
    /// Property type.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public Type PropertyType { get; set; } = null!;

    /// <summary>
    /// Entity model that contains property value for this field.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public object EntityModel { get; set; } = null!;

    private IReadOnlyDictionary<Type, InputType> TypeMappingDictionary { get; set; } = new Dictionary<Type, InputType>
    {
        { typeof(string), InputType.Text },
        { typeof(short), InputType.Number },
        { typeof(short?), InputType.Number },
        { typeof(ushort), InputType.Number },
        { typeof(ushort?), InputType.Number },
        { typeof(int), InputType.Number },
        { typeof(int?), InputType.Number },
        { typeof(uint), InputType.Number },
        { typeof(uint?), InputType.Number },
        { typeof(long), InputType.Number },
        { typeof(long?), InputType.Number },
        { typeof(ulong), InputType.Number },
        { typeof(ulong?), InputType.Number },
        { typeof(float), InputType.Number },
        { typeof(float?), InputType.Number },
        { typeof(double), InputType.Number },
        { typeof(double?), InputType.Number },
        { typeof(decimal), InputType.Number },
        { typeof(decimal?), InputType.Number },
        { typeof(DateTime), InputType.DateTimeLocal },
        { typeof(DateTime?), InputType.DateTimeLocal },
        { typeof(DateTimeOffset), InputType.DateTimeLocal },
        { typeof(DateTimeOffset?), InputType.DateTimeLocal },
        { typeof(DateOnly), InputType.Date },
        { typeof(DateOnly?), InputType.Date },
    };

    private IReadOnlyDictionary<List<Type>, InputType> TypeMappingDictionary2 { get; set; } = new Dictionary<List<Type>, InputType>
    {
        {
            new List<Type> { typeof(string) }, InputType.Text
        },
        {
            new List<Type>
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
            }, InputType.Number
        },
        {
            new List<Type>
            {
                typeof(DateTime), typeof(DateTime?),
                typeof(DateTimeOffset), typeof(DateTimeOffset?)
            }, InputType.DateTimeLocal
        },
        {
            new List<Type>
            {
                typeof(DateOnly), typeof(DateOnly?),
            }, InputType.Date

        },
    };

    public InputType GetInputType(Type propertyType)
    {
        foreach (var (types, inputType) in TypeMappingDictionary2)
        {
            if (types.Contains(propertyType))
            {
                return inputType;
            }
        }

        return InputType.Text;
    }

    /// <summary>
    /// Handles input changes.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="propertyName">Name of property that related to the input.</param>
    public void HandleInputChange(object value, string propertyName)
    {
        var property = EntityModel.GetType().GetProperty(propertyName)!;

        if (string.IsNullOrEmpty(value.ToString()))
        {
            property.SetValue(EntityModel, null);
            return;
        }

        var propertyType = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
        object convertedValue;
        if (propertyType == typeof(DateTimeOffset))
        {
            convertedValue = DateTimeOffset.Parse(value.ToString());
        }
        else if (propertyType == typeof(DateOnly))
        {
            convertedValue = DateOnly.Parse(value.ToString());
        }
        else if (propertyType.IsEnum)
        {
            convertedValue = Enum.Parse(propertyType, value.ToString());
        }
        else
        {
            convertedValue = Convert.ChangeType(value, propertyType);
        }

        property.SetValue(EntityModel, convertedValue);
    }
}