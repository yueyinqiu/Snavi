using System.Text.Json.Serialization;

namespace Snavi.Executing;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true
)]
[JsonSerializable(typeof(ArgumentProviderOutput))]
partial class ArgumentProviderOutputSerializerContext : JsonSerializerContext
{
}