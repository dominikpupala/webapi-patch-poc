using Asp.Versioning;
using System.Collections.Frozen;

namespace WebApiPatchPoC;

internal static class ApiVersions
{
    public static ApiVersion DefaultVersion => V1;

    public static ApiVersion V1 { get; } = new(1);

    public static ApiVersion V2 { get; } = new(2);

    public static FrozenDictionary<int, ApiVersion> Versions
        => field = new Dictionary<int, ApiVersion>
        {
            [1] = V1,
            [2] = V2
        }
        .ToFrozenDictionary();

    extension(ApiVersion apiVersion)
    {
        internal string ToDocumentName => $"v{apiVersion.MajorVersion}";
    }
}
