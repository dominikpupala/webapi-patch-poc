using System.Text.Json;
using WebApiPatchPoC;
using WebApiPatchPoC.Features.Products.PatchProduct;

namespace WebApiPatchPoC.Utility;

internal static class PatchMapper
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static (T Patch, HashSet<string> SetFields) Map<T>(JsonElement element) where T : new()
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException(
                $"Expected a JSON object for patch, but got {element.ValueKind}");
        }

        var patch = new T();
        var setFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var props = typeof(T)
            .GetProperties()
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var jsonProp in element.EnumerateObject())
        {
            var key = jsonProp.Name;

            if (!props.TryGetValue(key, out var prop))
            {
                continue;
            }

            var value = jsonProp.Value.ValueKind != JsonValueKind.Null
                ? JsonSerializer.Deserialize(
                    jsonProp.Value.GetRawText(),
                    prop.PropertyType,
                    _jsonOptions)
                : null;

            prop.SetValue(patch, value);
            setFields.Add(prop.Name);
        }

        return (patch, setFields);
    }
}
