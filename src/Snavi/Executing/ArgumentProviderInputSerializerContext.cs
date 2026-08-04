using System.Text.Json.Serialization;

namespace Snavi.Executing;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    RespectNullableAnnotations = true
)]
[JsonSerializable(typeof(ArgumentProviderInput))]
partial class ArgumentProviderInputSerializerContext : JsonSerializerContext
{
}