using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Users.WebAPI.Common.Converters
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        /// <summary>
        /// Reads and converts the JSON representation of a decimal value.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type of the object to convert to.</param>
        /// <param name="options">Options to use for deserialization.</param>
        /// <returns>The decimal value read from the JSON.</returns>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType.Equals(JsonTokenType.Number)) return reader.GetDecimal();

            string? value = reader.GetString();

            if(string.IsNullOrEmpty(value)) return 0;

            value = value.Replace(",", ".");

            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Writes a decimal value as a JSON number.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The decimal value to write.</param>
        /// <param name="options">Options to use for serialization.</param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
    }
}