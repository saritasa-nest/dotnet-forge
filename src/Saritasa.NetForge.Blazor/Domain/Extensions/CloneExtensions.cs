using Newtonsoft.Json;

namespace Saritasa.NetForge.Blazor.Domain.Extensions;

/// <summary>
/// Contains methods to make clone of objects.
/// </summary>
public static class CloneExtensions
{
    /// <summary>
    /// Perform a deep copy of the object, using Json as a serialization method.
    /// </summary>
    /// <remarks>
    /// Private members are not cloned using this method.
    /// </remarks>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static object? CloneJson(this object? source)
    {
        if (source is null)
        {
            return default;
        }

        // Initialize inner objects individually
        // for example in default constructor some list property initialized with some values,
        // but in 'source' these items are cleaned -
        // without ObjectCreationHandling.Replace default constructor values will be added to result
        var deserializeSettings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        var serializeSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        serializeSettings.Converters.Add(new DecimalJsonConverter());

        var serializedSource = JsonConvert.SerializeObject(source, serializeSettings);
        var sourceType = source.GetType();
        return JsonConvert.DeserializeObject(serializedSource, sourceType, deserializeSettings);
    }
}

/// <summary>
/// JSON converter for <c>decimal</c> type.
/// </summary>
/// <remarks>
/// Why we use this converter:
/// When we convert 1000 using JSON we will have 1000.0 value instead.
/// This converter prevents this behavior.
/// For example, it occurs when updating entity.
/// </remarks>
public class DecimalJsonConverter : JsonConverter<decimal>
{
    /// <summary>
    /// Non-implemented read method.
    /// </summary>
    public override decimal ReadJson(
        JsonReader reader, Type objectType, decimal existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, decimal value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
