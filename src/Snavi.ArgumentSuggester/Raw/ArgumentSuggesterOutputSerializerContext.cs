using System.Text.Json.Serialization;

namespace Snavi.ArgumentSuggester.Raw;

[JsonSourceGenerationOptions(
    AllowTrailingCommas = true,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    RespectNullableAnnotations = true,
    RespectRequiredConstructorParameters = true
)]
[JsonSerializable(typeof(ArgumentProviderOutput))]
public partial class ArgumentProviderOutputSerializerContext : JsonSerializerContext
{
}