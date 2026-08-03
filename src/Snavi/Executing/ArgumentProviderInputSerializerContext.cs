using System.Text.Json.Serialization;

namespace Snavi.Executing;

[JsonSerializable(typeof(ArgumentProviderInput))]
partial class ArgumentProviderInputSerializerContext : JsonSerializerContext
{
}