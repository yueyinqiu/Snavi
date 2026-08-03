using System.Text.Json.Serialization;

namespace Snavi.Executing;

[JsonSerializable(typeof(ArgumentProviderOutput))]
partial class ArgumentProviderOutputSerializerContext : JsonSerializerContext
{
}