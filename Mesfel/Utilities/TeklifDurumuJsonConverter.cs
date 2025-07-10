using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mesfel.Utilities
{
    public class TeklifDurumuJsonConverter : JsonConverter<TeklifDurumu>
    {
        public override TeklifDurumu Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return Enum.TryParse<TeklifDurumu>(value, true, out var result)
                ? result
                : TeklifDurumu.Verildi;
        }

        public override void Write(
            Utf8JsonWriter writer,
            TeklifDurumu value,
            JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
