using System;
using System.Text.Json;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;
using System.Linq;

public class CustomSystemTextJsonDestructuringPolicy : IDestructuringPolicy
{
    private readonly JsonSerializerOptions _options;

    public CustomSystemTextJsonDestructuringPolicy(JsonSerializerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
    {
        try
        {
            if (value == null)
            {
                result = new ScalarValue(null);
                return true;
            }

            var type = value.GetType();

            // Skip primitive types, strings, and common value types - let Serilog handle these normally
            if (type.IsPrimitive ||
                type == typeof(string) ||
                type == typeof(DateTime) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(TimeSpan) ||
                type == typeof(Guid) ||
                type == typeof(decimal))
            {
                result = null;
                return false;
            }

            // Check for empty collections before serialization
            if (IsEmptyCollection(value))
            {
                result = new ScalarValue(null);
                return true;
            }

            // For complex objects, serialize to JSON using System.Text.Json with custom options
            // This will respect your JsonIgnoreCondition.WhenWritingDefault setting
            var json = JsonSerializer.Serialize(value, type, _options);

            // If the JSON is just "{}" or "[]", we might want to skip it entirely
            if (json == "{}" || json == "[]" || json == "null")
            {
                result = new ScalarValue(null);
                return true;
            }

            // Return as a structured object by deserializing back to a dictionary/array
            // This allows Serilog to format it nicely in logs while still respecting our JSON options
            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                result = ConvertJsonElementToLogEventPropertyValue(jsonElement, propertyValueFactory);
                return true;
            }
            catch
            {
                // Fallback to raw JSON string if we can't convert back
                result = new ScalarValue(json);
                return true;
            }
        }
        catch
        {
            result = null;
            return false;
        }
    }

    private LogEventPropertyValue ConvertJsonElementToLogEventPropertyValue(JsonElement element, ILogEventPropertyValueFactory factory)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var properties = new List<LogEventProperty>();
                foreach (var property in element.EnumerateObject())
                {
                    // Skip empty values at the property level
                    if (ShouldSkipProperty(property.Value))
                        continue;

                    var propertyValue = ConvertJsonElementToLogEventPropertyValue(property.Value, factory);
                    properties.Add(new LogEventProperty(property.Name, propertyValue));
                }
                return new StructureValue(properties);

            case JsonValueKind.Array:
                // Skip empty arrays entirely
                if (element.GetArrayLength() == 0)
                    return new ScalarValue(null);

                var elements = element.EnumerateArray()
                    .Select(e => ConvertJsonElementToLogEventPropertyValue(e, factory))
                    .ToArray();
                return new SequenceValue(elements);

            case JsonValueKind.String:
                var stringValue = element.GetString();
                // Skip empty strings
                return string.IsNullOrEmpty(stringValue) ? new ScalarValue(null) : new ScalarValue(stringValue);

            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intValue))
                {
                    // Skip default int values (0)
                    return intValue == 0 ? new ScalarValue(null) : new ScalarValue(intValue);
                }
                if (element.TryGetInt64(out var longValue))
                {
                    // Skip default long values (0)
                    return longValue == 0 ? new ScalarValue(null) : new ScalarValue(longValue);
                }
                if (element.TryGetDouble(out var doubleValue))
                {
                    // Skip default double values (0.0)
                    return Math.Abs(doubleValue) < double.Epsilon ? new ScalarValue(null) : new ScalarValue(doubleValue);
                }
                return new ScalarValue(element.GetRawText());

            case JsonValueKind.True:
                return new ScalarValue(true);

            case JsonValueKind.False:
                // Skip default bool values (false) - comment out this line if you want to keep false values
                return new ScalarValue(null);
            // return new ScalarValue(false); // Uncomment to keep false values

            case JsonValueKind.Null:
                return new ScalarValue(null);

            default:
                return new ScalarValue(element.GetRawText());
        }
    }

    private bool ShouldSkipProperty(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                var stringValue = element.GetString();
                return string.IsNullOrEmpty(stringValue);

            case JsonValueKind.Array:
                return element.GetArrayLength() == 0;

            case JsonValueKind.Object:
                return element.EnumerateObject().Count() == 0;

            case JsonValueKind.Number:
                if (element.TryGetInt32(out var intValue))
                    return intValue == 0;
                if (element.TryGetInt64(out var longValue))
                    return longValue == 0;
                if (element.TryGetDouble(out var doubleValue))
                    return Math.Abs(doubleValue) < double.Epsilon;
                return false;

            case JsonValueKind.False:
                return true; // Skip false values - change to false if you want to keep them

            case JsonValueKind.Null:
                return true;

            default:
                return false;
        }
    }

    private bool IsEmptyCollection(object value)
    {
        if (value == null) return true;

        // Check for common collection types
        if (value is System.Collections.ICollection collection)
        {
            return collection.Count == 0;
        }

        // Check for IEnumerable (covers most collection types)
        if (value is System.Collections.IEnumerable enumerable)
        {
            // Check if enumerable is empty
            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        return false;
    }
}