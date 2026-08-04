using System.Text.Json.Serialization;

namespace Snavi.Modeling;

[JsonSerializable(typeof(CheatFile))]
partial class CheatFileSerializerContext : JsonSerializerContext
{
}