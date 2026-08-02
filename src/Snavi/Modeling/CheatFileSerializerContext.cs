using System.Text.Json.Serialization;
using Snavi.CheatModeling;

[JsonSerializable(typeof(CheatFile))]
partial class CheatFileSerializerContext : JsonSerializerContext
{
}